using System;
using System.Windows;
using System.Threading;
using Common;
using HDSInspector.SubWindow;

namespace HDSInspector
{
    /// <summary>   Laser process.  </summary>
    class LaserProcess
    {
        /// <summary> The pointer main window </summary>
        private MainWindow m_ptrMainWindow;

        /// <summary> The pointer inspection monitoring control </summary>
        private static InspectionMonitoring m_ptrInspectionMonitoringCtrl;
        private int m_TactTimeStart;
        private int m_TactTimeEnd;
        private double m_TotalTime;
        private int m_Count;
        private int m_BadCount;
        private bool bLaser;
        Thread Mark1;
        Thread Mark2;

        static public InspectionMonitoring PtrInspectionMonitoringCtrl
        {
            get
            {
                return m_ptrInspectionMonitoringCtrl;
            }
            set
            {
                m_ptrInspectionMonitoringCtrl = value;
            }
        }

        /// <summary>   Initializes a new instance of the LaserProcess class. </summary>
        public LaserProcess(out int val, bool bEnable)
        {
            m_ptrMainWindow = (MainWindow)Application.Current.MainWindow;
            if (m_ptrMainWindow != null)
            {
                m_ptrInspectionMonitoringCtrl = m_ptrMainWindow.InspectionMonitoringCtrl;
                val = 0;
            }
            else
            {
                val = 1; // Error
            }
            bLaser = bEnable;
        }

        /// <summary>   Markings this object. </summary>
        public void Marking()
        {
            Mark1 = new Thread(new ThreadStart(MarkProc1));
            Mark1.Start();
            if (MainWindow.Setting.SubSystem.Laser.DualLaser)
            {
                Mark2 = new Thread(new ThreadStart(MarkProc2));
                Mark2.Start();
            }
        }

        public void Stop()
        {
            if (Mark1 != null)
            {
                Mark1.Abort();
                Thread.Sleep(100);
                Mark1 = null;
            }
            if (Mark2 != null)
            {
                Mark2.Abort();
                Thread.Sleep(100);
                Mark2 = null;
            }
        }

        public void ResetTimer()
        {
            m_Count = 0;
            m_TotalTime = 0;
            m_BadCount = 0;
            Action a = delegate
            {
                m_ptrMainWindow.lblLMCycle.Content = string.Format("{0:F2}Sec", 0);
                m_ptrMainWindow.lblLMAverg.Content = string.Format("{0:F2}Sec", 0);
            }; m_ptrMainWindow.Dispatcher.Invoke(a);
        }



        /// <summary>   Mark procedure, Thread function. </summary>
        private void MarkProc1()
        {
            Action action = null;

            Action tackTimeChecker = delegate
            {
                double t = (m_TactTimeEnd - m_TactTimeStart) / (double)1000;
                m_Count++;
                m_ptrMainWindow.lblLMCycle.Content = string.Format("{0:F2}Sec", t);
                m_TotalTime += t;
                m_ptrMainWindow.lblLMAverg.Content = string.Format("{0:F2}Sec", m_TotalTime / m_Count);
                m_ptrInspectionMonitoringCtrl.ResultTable.txtXOut.Text = string.Format("{0:F2}", (double)m_BadCount / (double)m_Count);
            };

            int nBad = 0;
            int debugSequence = 0;
            while (m_ptrInspectionMonitoringCtrl.InspectionStarted)
            {
                debugSequence++;
                MainWindow.Log("Laser", SeverityLevel.INFO, "1 MarkProc1 debugSequence : " + debugSequence, true);

                while (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestAlign(0))
                {
                    if (!m_ptrInspectionMonitoringCtrl.InspectionStarted) return;
                    Thread.Sleep(100);
                }

                InspectProcess.Laser1_Done = false; // hs - Align 잡고 Laser 시작

                int plcID = m_ptrMainWindow.PCSInstance.PlcDevice.ReadLaser1ID();
                MainWindow.Log("Laser", SeverityLevel.INFO, "1 ReadLaser1ID : " + plcID, true);

                action = delegate { m_ptrInspectionMonitoringCtrl.lblMk1ID.Content = plcID.ToString("0000"); m_ptrInspectionMonitoringCtrl.InitializeMakingMap(0); }; m_ptrMainWindow.Dispatcher.Invoke(action);
                if (!bLaser || (MainWindow.CurrentModel.UseVerify && !MainWindow.CurrentModel.UseIDMark) || MainWindow.CurrentModel.UseAI)
                {
                    if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                    {
                        m_ptrMainWindow.PCSInstance.PlcDevice.ByPass(0);
                        continue;
                    }
                    else
                    {
                        m_ptrMainWindow.PCSInstance.PlcDevice.PassAlign(0);
                        Thread.Sleep(300);
                        while (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestAlign(0))
                        {
                            if (!m_ptrInspectionMonitoringCtrl.InspectionStarted) return;
                            Thread.Sleep(100);
                        }
                        m_ptrMainWindow.PCSInstance.PlcDevice.PassAlign(0);
                        Thread.Sleep(300);
                        for (int i = 0; i < MainWindow.CurrentModel.Strip.MarkStep; i++)
                        {
                            while (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestLaser(0))
                            {
                                if (!m_ptrInspectionMonitoringCtrl.InspectionStarted) return;
                                Thread.Sleep(100);
                            }
                            Thread.Sleep(100);
                            m_ptrMainWindow.PCSInstance.PlcDevice.PassLaser(0);
                        }
                        continue;
                    }
                }

                InspectBuffer tmp = new InspectBuffer();
                tmp = m_ptrInspectionMonitoringCtrl.LaserResult[m_ptrInspectionMonitoringCtrl.m_LaserPos % 20];
                nBad = tmp.BadCount();
                m_BadCount += nBad;
                m_ptrInspectionMonitoringCtrl.m_LaserPos++;
                if (m_ptrInspectionMonitoringCtrl.m_LaserPos > 1)
                {
                    m_TactTimeEnd = Environment.TickCount;
                    m_ptrMainWindow.Dispatcher.Invoke(tackTimeChecker);
                }
                m_TactTimeStart = Environment.TickCount;

                if (MainWindow.Setting.SubSystem.Laser.DualLaser) // 단독 보트일 경우 확인 불필요, 삭제시 인덱스 오류 발생
                {
                    if (tmp.ID != plcID)
                    {
                        action = delegate
                        {
                            m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                            m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                            m_ptrMainWindow.MainToolBarCtrl.InspectionStop(6);
                            m_ptrMainWindow.InspectionStarted = false;
                        };
                        m_ptrMainWindow.Dispatcher.Invoke(action);
                        MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Marking ID Dismatch)", true);
                        MessageBox.Show("PLC 마킹 ID 비정상. PLC 점검 필요합니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.");                       
                    }
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "1 InspcetAlignRight 진입전");
                Thread.Sleep(200);

                if (!m_ptrInspectionMonitoringCtrl.LaserDlg.InspcetAlignRight(0))
                {
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(true);

                    action = delegate
                    {
                        m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                        m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                        m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                        m_ptrMainWindow.InspectionStarted = false;
                        
                    };m_ptrMainWindow.Dispatcher.Invoke(action);
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Laser Align Fail)", true);
                    action = delegate
                    {
                        WarningDlg warningDlg = new WarningDlg("Laser AlignRight Failure. 자동 검사가 강제 종료되었습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.");
                        warningDlg.ShowDialog();
                    };m_ptrMainWindow.Dispatcher.Invoke(action);                
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(false);                   
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "1 InspcetAlignRight 성공!");

                m_ptrMainWindow.PCSInstance.PlcDevice.PassAlign(0);
                Thread.Sleep(100);

                MainWindow.Log("Laser", SeverityLevel.INFO, "1 PassAlign1!");

                //얼라인 요구 2
                while (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestAlign(0))
                {
                    if (!m_ptrInspectionMonitoringCtrl.InspectionStarted) return;
                    Thread.Sleep(100);
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "1 RequestAlign2 신호 받음!");
                
                Thread.Sleep(200);
                if (!m_ptrInspectionMonitoringCtrl.LaserDlg.InspcetAlignLeft(0))
                {
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(true);

                    action = delegate
                    {
                        m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                        m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                        m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                        m_ptrMainWindow.InspectionStarted = false;
                    };
                    m_ptrMainWindow.Dispatcher.Invoke(action);
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Laser Align Fail)", true);
                    action = delegate
                    {
                        WarningDlg warningDlg = new WarningDlg("Laser AlignLeft Failure. 자동 검사가 강제 종료되었습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.");
                        warningDlg.ShowDialog();
                    }; m_ptrMainWindow.Dispatcher.Invoke(action);    
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(false);                   
                }
                MainWindow.Log("Laser", SeverityLevel.INFO, "1 InspcetAlignLeft 성공!");

                m_ptrMainWindow.PCSInstance.PlcDevice.PassAlign(0);
                Thread.Sleep(100);

                MainWindow.Log("Laser", SeverityLevel.INFO, "1 PassAlign2!");

                int chkRequestLaser = 0;
                while (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestLaser(0)) // hs - Laser 요구
                {
                    if(!MainWindow.Setting.SubSystem.Laser.DualLaser && chkRequestLaser > 50)
                    {
                        action = delegate
                        {
                            m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                            m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                            m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                            m_ptrMainWindow.InspectionStarted = false;
                        };
                        m_ptrMainWindow.Dispatcher.Invoke(action);
                        MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Laser Align Fail)", true);
                        MessageBox.Show("RequestLaser Timeout !!");                      
                    }

                    if (!m_ptrInspectionMonitoringCtrl.InspectionStarted) return;
                    Thread.Sleep(100);
                    chkRequestLaser++;
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "1 레이저 요구 받음!");
                int XoutCount = 0;
                action = delegate
                {
                    m_ptrInspectionMonitoringCtrl.Mark1Map.GoodColor();
                    int val = MainWindow.NG_Info.BadNameToID("Skip불량");
                    for (int i = 0; i < tmp.SizeX; i++)
                    {
                        for (int j = 0; j < tmp.SizeY; j++)
                        {
                            if (tmp.Get(i, j) == val)
                            {
                                XoutCount++;
                                m_ptrInspectionMonitoringCtrl.Mark1Map.SetColor(i, j, System.Windows.Media.Colors.Black);
                            }
                                
                            else if (tmp.Get(i, j) == 9 || tmp.Get(i, j) == 10)
                            {
                                XoutCount++;
                                m_ptrInspectionMonitoringCtrl.Mark1Map.SetColor(i, j, System.Windows.Media.Colors.Olive);
                            }
                                
                            else
                            {
                                if (tmp.Get(i, j) != 0)
                                {
                                    XoutCount++;
                                    m_ptrInspectionMonitoringCtrl.Mark1Map.SetColor(i, j, System.Windows.Media.Colors.Orange);
                                }
                            }
                        }
                    }
                    m_ptrInspectionMonitoringCtrl.Mark1Map.SetStripMap_XOutCount(XoutCount);
                }; m_ptrMainWindow.Dispatcher.Invoke(action);

                if (!m_ptrInspectionMonitoringCtrl.LaserDlg.RunLaser(tmp, 0, m_ptrInspectionMonitoringCtrl.m_LaserPos + MainWindow.OldMarkID, MainWindow.CurrentModel.UseVerify))
                {
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(true);
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Marking Fail)", true); // hs - 의심코드
                    Thread.Sleep(100);

                    action = delegate
                    {
                        m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                        m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                        m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                        m_ptrMainWindow.InspectionStarted = false;
                    };
                    m_ptrMainWindow.Dispatcher.Invoke(action);
                    action = delegate
                    {
                        WarningDlg warningDlg = new WarningDlg("RunLaser 실패, Marking Failure. 자동 검사가 강제 종료되었습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.\n레이저 문제 입니다. 설비에 문의하세요.");
                        warningDlg.ShowDialog();
                    }; m_ptrMainWindow.Dispatcher.Invoke(action);
                    
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(false);
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "1 RunLaser 성공!");

                m_ptrInspectionMonitoringCtrl.CreateIDMarkMap(tmp, m_ptrInspectionMonitoringCtrl.m_LaserPos + MainWindow.OldMarkID);
                tmp.Clear();
                Thread.Sleep(300);

                InspectProcess.Laser1_Done = true;

                //양품 마킹 끝나고 종료 확인, 넣어 줄 때는 m_Lase rPos가 증가하기 전에 넣어줌 : m_LaserPos - 1
                // hs - Laser1, Laser2 모두 끝나야 오더 종료할 수 있게끔 조건 추가
                // hs - 삭제
                #region
                //if (/*(InspectProcess.Laser_Done && InspectProcess.Laser2_Done)*/ InspectProcess.Laser_Done && InspectProcess.Result_Done && InspectProcess.Loader_Done && (InspectProcess.globalStripID == m_ptrInspectionMonitoringCtrl.LaserResult[(m_ptrInspectionMonitoringCtrl.m_LaserPos - 1) % 20].ID))
                //{
                //    Reinspect();
                //}
                #endregion
            }
        }

        private void MarkProc2()
        {   // hs - dual Laser 
            Action action = null;
            Action tackTimeChecker = delegate
            {
                double t = (m_TactTimeEnd - m_TactTimeStart) / (double)1000;
                m_Count++;
                m_ptrMainWindow.lblLMCycle.Content = string.Format("{0:F2}Sec", t);
                m_TotalTime += t;
                m_ptrMainWindow.lblLMAverg.Content = string.Format("{0:F2}Sec", m_TotalTime / m_Count);
                m_ptrInspectionMonitoringCtrl.ResultTable.txtXOut.Text = string.Format("{0:F2}", (double)m_BadCount / (double)m_Count);
            };
            int nBad = 0;
            while (m_ptrInspectionMonitoringCtrl.InspectionStarted)
            {
                //Align1
                while (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestAlign(1))
                {
                    if (!m_ptrInspectionMonitoringCtrl.InspectionStarted) return;
                    Thread.Sleep(100);
                }

                InspectProcess.Laser2_Done = false; // hs - 여기 타이밍 체크 (마킹중)

                int plcID = m_ptrMainWindow.PCSInstance.PlcDevice.ReadLaser2ID();
                action = delegate { m_ptrInspectionMonitoringCtrl.lblMk2ID.Content = plcID.ToString("0000"); m_ptrInspectionMonitoringCtrl.InitializeMakingMap(1); }; m_ptrMainWindow.Dispatcher.Invoke(action);
                if (!bLaser || (MainWindow.CurrentModel.UseVerify && !MainWindow.CurrentModel.UseIDMark) || MainWindow.CurrentModel.UseAI)
                {
                    m_ptrMainWindow.PCSInstance.PlcDevice.ByPass(1);
                    continue;
                }
                InspectBuffer tmp = new InspectBuffer();
                tmp = m_ptrInspectionMonitoringCtrl.LaserResult[m_ptrInspectionMonitoringCtrl.m_LaserPos % 20];
                nBad = tmp.BadCount();
                m_BadCount += nBad;
                m_ptrInspectionMonitoringCtrl.m_LaserPos++;
                if (m_ptrInspectionMonitoringCtrl.m_LaserPos > 1)
                {
                    m_TactTimeEnd = Environment.TickCount;
                    m_ptrMainWindow.Dispatcher.Invoke(tackTimeChecker);
                }
                m_TactTimeStart = Environment.TickCount;
                if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                {
                    if (tmp.ID != plcID)
                    {
                        MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Marking ID Dismatch)", true);
                        m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(true);
                        Thread.Sleep(100);

                        action = delegate
                        {
                            m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                            m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                            m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                            m_ptrMainWindow.InspectionStarted = false;
                        }; m_ptrMainWindow.Dispatcher.Invoke(action);

                        action = delegate
                        {
                            WarningDlg warningDlg = new WarningDlg("Marking Failure(시퀀스 불일치). 자동 검사가 강제 종료되었습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.");
                            warningDlg.ShowDialog();
                        }; m_ptrMainWindow.Dispatcher.Invoke(action);

                        m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(false);
                    }
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "2 InspcetAlignRight 진입전");
                Thread.Sleep(200);

                if (!m_ptrInspectionMonitoringCtrl.LaserDlg.InspcetAlignRight(1))
                {
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(true);

                    action = delegate
                    {
                        m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                        m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                        m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                        m_ptrMainWindow.InspectionStarted = false;
                    };
                    m_ptrMainWindow.Dispatcher.Invoke(action);
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Laser Align Fail)", true);
                    action = delegate
                    {
                        WarningDlg warningDlg = new WarningDlg("Laser AlignRight Failure. 자동 검사가 강제 종료되었습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.");
                        warningDlg.ShowDialog();
                    }; m_ptrMainWindow.Dispatcher.Invoke(action);
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(false);                   
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "2 InspcetAlignRight 성공!");

                if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                    m_ptrMainWindow.PCSInstance.PlcDevice.PassAlign(1);
                Thread.Sleep(100);

                MainWindow.Log("Laser", SeverityLevel.INFO, "2 PassAlign1!");

                //Align2
                while (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestAlign(1))
                {
                    if (!m_ptrInspectionMonitoringCtrl.InspectionStarted) return;
                    Thread.Sleep(100);
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "2 RequestAlign2 신호 받음!");

                Thread.Sleep(200);
                if (!m_ptrInspectionMonitoringCtrl.LaserDlg.InspcetAlignLeft(1))
                {
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(true);

                    action = delegate
                    {
                        m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                        m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                        m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                        m_ptrMainWindow.InspectionStarted = false;
                    };
                    m_ptrMainWindow.Dispatcher.Invoke(action);
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Laser Align Fail)", true);
                    action = delegate
                    {
                        WarningDlg warningDlg = new WarningDlg("Laser AlignLeft Failure. 자동 검사가 강제 종료되었습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.");
                        warningDlg.ShowDialog();
                    }; m_ptrMainWindow.Dispatcher.Invoke(action);                
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(false);                  
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "2 InspcetAlignLeft 성공!");

                if (MainWindow.Setting.SubSystem.Laser.DualLaser)
                    m_ptrMainWindow.PCSInstance.PlcDevice.PassAlign(1);
                Thread.Sleep(100);

                MainWindow.Log("Laser", SeverityLevel.INFO, "2 PassAlign 성공!");

                while (!m_ptrMainWindow.PCSInstance.PlcDevice.RequestLaser(1))
                {
                    if (!m_ptrInspectionMonitoringCtrl.InspectionStarted) return;
                    Thread.Sleep(100);
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "2 RequestLaser 신호 받음!");
                int XoutCount = 0;
                action = delegate
                {
                    m_ptrInspectionMonitoringCtrl.Mark2Map.GoodColor();
                    int val = MainWindow.NG_Info.BadNameToID("Skip불량");
                    for (int i = 0; i < tmp.SizeX; i++)
                    {
                        for (int j = 0; j < tmp.SizeY; j++)
                        {
                            if (tmp.Get(i, j) == val)
                            {
                                XoutCount++;
                                m_ptrInspectionMonitoringCtrl.Mark2Map.SetColor(i, j, System.Windows.Media.Colors.Black);
                            }
                            else if (tmp.Get(i, j) == 9 || tmp.Get(i, j) == 10)
                            {
                                XoutCount++;
                                m_ptrInspectionMonitoringCtrl.Mark2Map.SetColor(i, j, System.Windows.Media.Colors.Olive);
                            }
                            else
                            {
                                if (tmp.Get(i, j) != 0)
                                {
                                    XoutCount++;
                                    m_ptrInspectionMonitoringCtrl.Mark2Map.SetColor(i, j, System.Windows.Media.Colors.Orange);
                                }
                            }
                        }
                    }
                    m_ptrInspectionMonitoringCtrl.Mark2Map.SetStripMap_XOutCount(XoutCount);
                }; m_ptrMainWindow.Dispatcher.Invoke(action);

                if (!m_ptrInspectionMonitoringCtrl.LaserDlg.RunLaser(tmp, 1, m_ptrInspectionMonitoringCtrl.m_LaserPos + MainWindow.OldMarkID, MainWindow.CurrentModel.UseVerify))
                {
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(true);
                    MainWindow.Log("PCS", SeverityLevel.FATAL, "자동 검사가 강제 종료되었습니다. (Marking Fail)", true);
                    Thread.Sleep(100);

                    action = delegate
                    {
                        m_ptrInspectionMonitoringCtrl.InspectionStarted = false;
                        m_ptrInspectionMonitoringCtrl.InspectionThreadStarted = false;
                        m_ptrMainWindow.MainToolBarCtrl.InspectionStop(5);
                        m_ptrMainWindow.InspectionStarted = false;
                    };
                    m_ptrMainWindow.Dispatcher.Invoke(action);

                    action = delegate
                    {
                        WarningDlg warningDlg = new WarningDlg("RunLaser 실패, Marking Failure. 자동 검사가 강제 종료되었습니다.\n원점복귀 후 다시 시작해 주시기 바랍니다.\n레이저 문제 입니다. 설비에 문의하세요.");
                        warningDlg.ShowDialog();
                    }; m_ptrMainWindow.Dispatcher.Invoke(action);

                    
                    m_ptrMainWindow.PCSInstance.PlcDevice.SetWarning(false);
                }

                MainWindow.Log("Laser", SeverityLevel.INFO, "2 RunLaser 성공!");

                m_ptrInspectionMonitoringCtrl.CreateIDMarkMap(tmp, m_ptrInspectionMonitoringCtrl.m_LaserPos + MainWindow.OldMarkID);
                tmp.Clear();

                Thread.Sleep(300);

                InspectProcess.Laser2_Done = true;

                // hs - Laser1, Laser2 모두 끝나야 오더 종료할 수 있게끔 조건 추가, 오더 종료 한곳에서 관리되도록
                //if (/*(InspectProcess.Laser_Done && InspectProcess.Laser2_Done)*/ InspectProcess.Laser1_Done && InspectProcess.Result_Done && InspectProcess.Loader_Done && 
                //    (InspectProcess.globalStripID == m_ptrInspectionMonitoringCtrl.LaserResult[(m_ptrInspectionMonitoringCtrl.m_LaserPos - 1) % 20].ID))
                //{
                //    Reinspect();
                //}
            }
        }

        private void Reinspect()
        {
            m_ptrMainWindow.InspectionMonitoringCtrl.EndLoader = true;
            Action action = delegate
            {
                m_ptrMainWindow.EndInspect.Visibility = Visibility.Visible;
                m_ptrMainWindow.EndInspect.StartTimer();
                m_ptrMainWindow.MainToolBarCtrl.IsEnabled = false;
            };
            m_ptrMainWindow.Dispatcher.Invoke(action);
            while (m_ptrMainWindow.InspectionMonitoringCtrl.EndLoader)
            {
                Thread.Sleep(100);
                if (!m_ptrMainWindow.InspectionMonitoringCtrl.InspectionStarted)
                {
                    MainWindow.Log("PCS", SeverityLevel.DEBUG, "자동 검사가 종료되었습니다. (제품 이송)");
                    m_ptrMainWindow.InspectionMonitoringCtrl.InspectionThreadStarted = false;
                    InspectProcess.LightOff(3);
                    InspectProcess.Loader_Done = false;
                    return;
                }
            }
            InspectProcess.Loader_Done = false;
        }
    }
}
