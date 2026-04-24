using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DNN
{
    public struct Iter_Data
    {
        public int iter;
        public int max_batch;
        public int best_iter;
        public float avg_loss;
        public float time_left;
        public float mAP;
        public float best;
    }

    public class YoloV4
    {
        object[] yolos = new object[42];
        YoloV4_01 yolo_01;
        YoloV4_02 yolo_02;
        YoloV4_03 yolo_03;
        YoloV4_04 yolo_04;
        YoloV4_05 yolo_05;
        YoloV4_06 yolo_06;
        YoloV4_07 yolo_07;
        YoloV4_08 yolo_08;
        YoloV4_09 yolo_09;
        YoloV4_10 yolo_10;
        YoloV4_11 yolo_11;
        YoloV4_12 yolo_12;
        YoloV4_13 yolo_13;
        YoloV4_14 yolo_14;
        YoloV4_15 yolo_15;
        YoloV4_16 yolo_16;
        YoloV4_17 yolo_17;
        YoloV4_18 yolo_18;
        YoloV4_19 yolo_19;
        YoloV4_20 yolo_20;
        YoloV4_21 yolo_21;
        YoloV4_22 yolo_22;
        YoloV4_23 yolo_23;
        YoloV4_24 yolo_24;
        YoloV4_25 yolo_25;
        YoloV4_26 yolo_26;
        YoloV4_27 yolo_27;
        YoloV4_28 yolo_28;
        YoloV4_29 yolo_29;
        YoloV4_30 yolo_30;
        YoloV4_31 yolo_31;
        YoloV4_32 yolo_32;
        YoloV4_33 yolo_33;
        YoloV4_34 yolo_34;
        YoloV4_35 yolo_35;
        YoloV4_36 yolo_36;
        YoloV4_37 yolo_37;
        YoloV4_38 yolo_38;
        YoloV4_39 yolo_39;
        YoloV4_40 yolo_40;
        YoloV4_41 yolo_41;
        YoloV4_42 yolo_42;
        public YoloV4()
        {
            yolos[0] = (object)(yolo_01);
            yolos[1] = (object)(yolo_02);
            yolos[2] = (object)(yolo_03);
            yolos[3] = (object)(yolo_04);
            yolos[4] = (object)(yolo_05);
            yolos[5] = (object)(yolo_06);
            yolos[6] = (object)(yolo_07);
            yolos[7] = (object)(yolo_08);
            yolos[8] = (object)(yolo_09);
            yolos[9] = (object)(yolo_10);
            yolos[10] = (object)(yolo_11);
            yolos[11] = (object)(yolo_12);
            yolos[12] = (object)(yolo_13);
            yolos[13] = (object)(yolo_14);
            yolos[14] = (object)(yolo_15);
            yolos[15] = (object)(yolo_16);
            yolos[16] = (object)(yolo_17);
            yolos[17] = (object)(yolo_18);
            yolos[18] = (object)(yolo_19);
            yolos[19] = (object)(yolo_20);
            yolos[20] = (object)(yolo_21);
            yolos[21] = (object)(yolo_22);
            yolos[22] = (object)(yolo_23);
            yolos[23] = (object)(yolo_24);
            yolos[24] = (object)(yolo_25);
            yolos[25] = (object)(yolo_26);
            yolos[26] = (object)(yolo_27);
            yolos[27] = (object)(yolo_28);
            yolos[28] = (object)(yolo_29);
            yolos[29] = (object)(yolo_30);
            yolos[30] = (object)(yolo_31);
            yolos[31] = (object)(yolo_32);
            yolos[32] = (object)(yolo_33);
            yolos[33] = (object)(yolo_34);
            yolos[34] = (object)(yolo_35);
            yolos[35] = (object)(yolo_36);
            yolos[36] = (object)(yolo_37);
            yolos[37] = (object)(yolo_38);
            yolos[38] = (object)(yolo_39);
            yolos[39] = (object)(yolo_40);
            yolos[40] = (object)(yolo_41);
            yolos[41] = (object)(yolo_42);
        }

        public void Init(int index, string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {
            switch (index)
            {
                case 0: yolos[0] = new YoloV4_01(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 1: yolos[1] = new YoloV4_02(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 2: yolos[2] = new YoloV4_03(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 3: yolos[3] = new YoloV4_04(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 4: yolos[4] = new YoloV4_05(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 5: yolos[5] = new YoloV4_06(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 6: yolos[6] = new YoloV4_07(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 7: yolos[7] = new YoloV4_08(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 8: yolos[8] = new YoloV4_09(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 9: yolos[9] = new YoloV4_10(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 10: yolos[10] = new YoloV4_11(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 11: yolos[11] = new YoloV4_12(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 12: yolos[12] = new YoloV4_13(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 13: yolos[13] = new YoloV4_14(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 14: yolos[14] = new YoloV4_15(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 15: yolos[15] = new YoloV4_16(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 16: yolos[16] = new YoloV4_17(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 17: yolos[17] = new YoloV4_18(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 18: yolos[18] = new YoloV4_19(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 19: yolos[19] = new YoloV4_20(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 20: yolos[20] = new YoloV4_21(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 21: yolos[21] = new YoloV4_22(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 22: yolos[22] = new YoloV4_23(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 23: yolos[23] = new YoloV4_24(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 24: yolos[24] = new YoloV4_25(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 25: yolos[25] = new YoloV4_26(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 26: yolos[26] = new YoloV4_27(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 27: yolos[27] = new YoloV4_28(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 28: yolos[28] = new YoloV4_29(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 29: yolos[29] = new YoloV4_30(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 30: yolos[30] = new YoloV4_31(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 31: yolos[31] = new YoloV4_32(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 32: yolos[32] = new YoloV4_33(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 33: yolos[33] = new YoloV4_34(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 34: yolos[34] = new YoloV4_35(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 35: yolos[35] = new YoloV4_36(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 36: yolos[36] = new YoloV4_37(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 37: yolos[37] = new YoloV4_38(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 38: yolos[38] = new YoloV4_39(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 39: yolos[39] = new YoloV4_40(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 40: yolos[40] = new YoloV4_41(configurationFilename, weightsFilename, gpu, batch_size); break;
                case 41: yolos[41] = new YoloV4_42(configurationFilename, weightsFilename, gpu, batch_size); break;
            }

        }

        public void Dispose()
        {
            for (int i = 0; i < yolos.Length; i++)
            {
                if (yolos[i] != null)
                {
                    switch (i)
                    {
                        case 0: ((YoloV4_01)yolos[i]).Dispose(); break;
                        case 1: ((YoloV4_02)yolos[i]).Dispose(); break;
                        case 2: ((YoloV4_03)yolos[i]).Dispose(); break;
                        case 3: ((YoloV4_04)yolos[i]).Dispose(); break;
                        case 4: ((YoloV4_05)yolos[i]).Dispose(); break;
                        case 5: ((YoloV4_06)yolos[i]).Dispose(); break;
                        case 6: ((YoloV4_07)yolos[i]).Dispose(); break;
                        case 7: ((YoloV4_08)yolos[i]).Dispose(); break;
                        case 8: ((YoloV4_09)yolos[i]).Dispose(); break;
                        case 9: ((YoloV4_10)yolos[i]).Dispose(); break;
                        case 10: ((YoloV4_11)yolos[i]).Dispose(); break;
                        case 11: ((YoloV4_12)yolos[i]).Dispose(); break;
                        case 12: ((YoloV4_13)yolos[i]).Dispose(); break;
                        case 13: ((YoloV4_14)yolos[i]).Dispose(); break;
                        case 14: ((YoloV4_15)yolos[i]).Dispose(); break;
                        case 15: ((YoloV4_16)yolos[i]).Dispose(); break;
                        case 16: ((YoloV4_17)yolos[i]).Dispose(); break;
                        case 17: ((YoloV4_18)yolos[i]).Dispose(); break;
                        case 18: ((YoloV4_19)yolos[i]).Dispose(); break;
                        case 19: ((YoloV4_20)yolos[i]).Dispose(); break;
                        case 20: ((YoloV4_21)yolos[i]).Dispose(); break;
                        case 21: ((YoloV4_22)yolos[i]).Dispose(); break;
                        case 22: ((YoloV4_23)yolos[i]).Dispose(); break;
                        case 23: ((YoloV4_24)yolos[i]).Dispose(); break;
                        case 24: ((YoloV4_25)yolos[i]).Dispose(); break;
                        case 25: ((YoloV4_26)yolos[i]).Dispose(); break;
                        case 26: ((YoloV4_27)yolos[i]).Dispose(); break;
                        case 27: ((YoloV4_28)yolos[i]).Dispose(); break;
                        case 28: ((YoloV4_29)yolos[i]).Dispose(); break;
                        case 29: ((YoloV4_30)yolos[i]).Dispose(); break;
                        case 30: ((YoloV4_31)yolos[i]).Dispose(); break;
                        case 31: ((YoloV4_32)yolos[i]).Dispose(); break;
                        case 32: ((YoloV4_33)yolos[i]).Dispose(); break;
                        case 33: ((YoloV4_34)yolos[i]).Dispose(); break;
                        case 34: ((YoloV4_35)yolos[i]).Dispose(); break;
                        case 35: ((YoloV4_36)yolos[i]).Dispose(); break;
                        case 36: ((YoloV4_37)yolos[i]).Dispose(); break;
                        case 37: ((YoloV4_38)yolos[i]).Dispose(); break;
                        case 38: ((YoloV4_39)yolos[i]).Dispose(); break;
                        case 39: ((YoloV4_40)yolos[i]).Dispose(); break;
                        case 40: ((YoloV4_41)yolos[i]).Dispose(); break;
                        case 41: ((YoloV4_42)yolos[i]).Dispose(); break;

                    }
                    yolos[i] = null;
                }
            }

        }

        public bbox_t[] Detect(int index, string filename)
        {
            bbox_t[] tmp = new bbox_t[1];
            switch (index)
            {
                case 0: tmp = ((YoloV4_01)yolos[0]).Detect(filename); break;
                case 1: tmp = ((YoloV4_02)yolos[1]).Detect(filename); break;
                case 2: tmp = ((YoloV4_03)yolos[2]).Detect(filename); break;
                case 3: tmp = ((YoloV4_04)yolos[3]).Detect(filename); break;
                case 4: tmp = ((YoloV4_05)yolos[4]).Detect(filename); break;
                case 5: tmp = ((YoloV4_06)yolos[5]).Detect(filename); break;
                case 6: tmp = ((YoloV4_07)yolos[6]).Detect(filename); break;
                case 7: tmp = ((YoloV4_08)yolos[7]).Detect(filename); break;
                case 8: tmp = ((YoloV4_09)yolos[8]).Detect(filename); break;
                case 9: tmp = ((YoloV4_10)yolos[9]).Detect(filename); break;
                case 10: tmp = ((YoloV4_11)yolos[10]).Detect(filename); break;
                case 11: tmp = ((YoloV4_12)yolos[11]).Detect(filename); break;
                case 12: tmp = ((YoloV4_13)yolos[12]).Detect(filename); break;
                case 13: tmp = ((YoloV4_14)yolos[13]).Detect(filename); break;
                case 14: tmp = ((YoloV4_15)yolos[14]).Detect(filename); break;
                case 15: tmp = ((YoloV4_16)yolos[15]).Detect(filename); break;
                case 16: tmp = ((YoloV4_17)yolos[16]).Detect(filename); break;
                case 17: tmp = ((YoloV4_18)yolos[17]).Detect(filename); break;
                case 18: tmp = ((YoloV4_19)yolos[18]).Detect(filename); break;
                case 19: tmp = ((YoloV4_20)yolos[19]).Detect(filename); break;
                case 20: tmp = ((YoloV4_21)yolos[20]).Detect(filename); break;
                case 21: tmp = ((YoloV4_22)yolos[21]).Detect(filename); break;
                case 22: tmp = ((YoloV4_23)yolos[22]).Detect(filename); break;
                case 23: tmp = ((YoloV4_24)yolos[23]).Detect(filename); break;
                case 24: tmp = ((YoloV4_25)yolos[24]).Detect(filename); break;
                case 25: tmp = ((YoloV4_26)yolos[25]).Detect(filename); break;
                case 26: tmp = ((YoloV4_27)yolos[26]).Detect(filename); break;
                case 27: tmp = ((YoloV4_28)yolos[27]).Detect(filename); break;
                case 28: tmp = ((YoloV4_29)yolos[28]).Detect(filename); break;
                case 29: tmp = ((YoloV4_30)yolos[29]).Detect(filename); break;
                case 30: tmp = ((YoloV4_31)yolos[30]).Detect(filename); break;
                case 31: tmp = ((YoloV4_32)yolos[31]).Detect(filename); break;
                case 32: tmp = ((YoloV4_33)yolos[32]).Detect(filename); break;
                case 33: tmp = ((YoloV4_34)yolos[33]).Detect(filename); break;
                case 34: tmp = ((YoloV4_35)yolos[34]).Detect(filename); break;
                case 35: tmp = ((YoloV4_36)yolos[35]).Detect(filename); break;
                case 36: tmp = ((YoloV4_37)yolos[36]).Detect(filename); break;
                case 37: tmp = ((YoloV4_38)yolos[37]).Detect(filename); break;
                case 38: tmp = ((YoloV4_39)yolos[38]).Detect(filename); break;
                case 39: tmp = ((YoloV4_40)yolos[39]).Detect(filename); break;
                case 40: tmp = ((YoloV4_41)yolos[40]).Detect(filename); break;
                case 41: tmp = ((YoloV4_42)yolos[41]).Detect(filename); break;
            }
            return tmp;
        }

        public bbox_t[] Detect(int index, byte[] imgs)
        {
            bbox_t[] tmp = new bbox_t[1];
            switch (index)
            {
                case 0: tmp = ((YoloV4_01)yolos[0]).Detect(imgs); break;
                case 1: tmp = ((YoloV4_02)yolos[1]).Detect(imgs); break;
                case 2: tmp = ((YoloV4_03)yolos[2]).Detect(imgs); break;
                case 3: tmp = ((YoloV4_04)yolos[3]).Detect(imgs); break;
                case 4: tmp = ((YoloV4_05)yolos[4]).Detect(imgs); break;
                case 5: tmp = ((YoloV4_06)yolos[5]).Detect(imgs); break;
                case 6: tmp = ((YoloV4_07)yolos[6]).Detect(imgs); break;
                case 7: tmp = ((YoloV4_08)yolos[7]).Detect(imgs); break;
                case 8: tmp = ((YoloV4_09)yolos[8]).Detect(imgs); break;
                case 9: tmp = ((YoloV4_10)yolos[9]).Detect(imgs); break;
                case 10: tmp = ((YoloV4_11)yolos[10]).Detect(imgs); break;
                case 11: tmp = ((YoloV4_12)yolos[11]).Detect(imgs); break;
                case 12: tmp = ((YoloV4_13)yolos[12]).Detect(imgs); break;
                case 13: tmp = ((YoloV4_14)yolos[13]).Detect(imgs); break;
                case 14: tmp = ((YoloV4_15)yolos[14]).Detect(imgs); break;
                case 15: tmp = ((YoloV4_16)yolos[15]).Detect(imgs); break;
                case 16: tmp = ((YoloV4_17)yolos[16]).Detect(imgs); break;
                case 17: tmp = ((YoloV4_18)yolos[17]).Detect(imgs); break;
                case 18: tmp = ((YoloV4_19)yolos[18]).Detect(imgs); break;
                case 19: tmp = ((YoloV4_20)yolos[19]).Detect(imgs); break;
                case 20: tmp = ((YoloV4_21)yolos[20]).Detect(imgs); break;
                case 21: tmp = ((YoloV4_22)yolos[21]).Detect(imgs); break;
                case 22: tmp = ((YoloV4_23)yolos[22]).Detect(imgs); break;
                case 23: tmp = ((YoloV4_24)yolos[23]).Detect(imgs); break;
                case 24: tmp = ((YoloV4_25)yolos[24]).Detect(imgs); break;
                case 25: tmp = ((YoloV4_26)yolos[25]).Detect(imgs); break;
                case 26: tmp = ((YoloV4_27)yolos[26]).Detect(imgs); break;
                case 27: tmp = ((YoloV4_28)yolos[27]).Detect(imgs); break;
                case 28: tmp = ((YoloV4_29)yolos[28]).Detect(imgs); break;
                case 29: tmp = ((YoloV4_30)yolos[29]).Detect(imgs); break;
                case 30: tmp = ((YoloV4_31)yolos[30]).Detect(imgs); break;
                case 31: tmp = ((YoloV4_32)yolos[31]).Detect(imgs); break;
                case 32: tmp = ((YoloV4_33)yolos[32]).Detect(imgs); break;
                case 33: tmp = ((YoloV4_34)yolos[33]).Detect(imgs); break;
                case 34: tmp = ((YoloV4_35)yolos[34]).Detect(imgs); break;
                case 35: tmp = ((YoloV4_36)yolos[35]).Detect(imgs); break;
                case 36: tmp = ((YoloV4_37)yolos[36]).Detect(imgs); break;
                case 37: tmp = ((YoloV4_38)yolos[37]).Detect(imgs); break;
                case 38: tmp = ((YoloV4_39)yolos[38]).Detect(imgs); break;
                case 39: tmp = ((YoloV4_40)yolos[39]).Detect(imgs); break;
                case 40: tmp = ((YoloV4_41)yolos[30]).Detect(imgs); break;
                case 41: tmp = ((YoloV4_42)yolos[41]).Detect(imgs); break;
            }
            return tmp;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct bbox_t
    {
        public UInt32 x, y, w, h;    // (x,y) - top-left corner, (w, h) - width & height of bounded box
        public float prob;           // confidence - probability that the object was found correctly
        public UInt32 obj_id;        // class of object - from range [0, classes-1]
        public UInt32 track_id;      // tracking id for video (0 - untracked, 1 - inf - tracked object)
        public UInt32 frames_counter;
        public float x_3d, y_3d, z_3d;  // 3-D coordinates, if there is used 3D-stereo camera
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct BboxContainer
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public bbox_t[] candidates;
    }

    public class YoloV4_01 : IDisposable
    {
        #region dll
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_01.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int nSize, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        #endregion

        public YoloV4_01(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {
            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);
        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }



        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_02 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_02.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_02(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_03 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_03.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_03(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_04 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_04.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_04(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_05 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_05.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_05(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_06 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_06.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_06(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_07 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_07.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();


        public YoloV4_07(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_08 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_08.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_08(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_09 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_09.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();


        public YoloV4_09(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_10 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_10.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_10(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_11 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_11.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_11(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_12 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_12.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_12(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_13 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_13.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_13(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_14 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_14.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();
        public YoloV4_14(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_15 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_15.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_15(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_16 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_16.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_16(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_17 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_17.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_17(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_18 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_18.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_18(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_19 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_19.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_19(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_20 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_20.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_20(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_21 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_21.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_21(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_22 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_22.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_22(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_23 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_23.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_23(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_24 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_24.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_24(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_25 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_25.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_25(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_26 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_26.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_26(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_27 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_27.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_27(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_28 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_28.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_28(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_29 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_29.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_29(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_30 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_30.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_30(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_31 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_31.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_31(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_32 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_32.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_32(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_33 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_33.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_33(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_34 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_34.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_34(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_35 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_35.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_35(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_36 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_36.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_36(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_37 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_37.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_37(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_38 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_38.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_38(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_39 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_39.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_39(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_40 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_40.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_40(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_41 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_41.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_41(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
               // System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

    public class YoloV4_42 : IDisposable
    {
        private const string YoloLibraryName = "\\yolo_dll\\yolo_cpp_dll_42.dll";
        private const int MaxObjects = 10;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int data_length, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [DllImport(YoloLibraryName, EntryPoint = "train_custom")]
        public static extern void train(string dataFilePath, string cfgFilePath, string weightsFile, int calc_map, int show_imgs);

        [DllImport(YoloLibraryName, EntryPoint = "get_iteration_data")]
        public static extern Iter_Data GetIterationData();

        public YoloV4_42(string configurationFilename, string weightsFilename, int gpu, int batch_size)
        {

            InitializeYolo(configurationFilename, weightsFilename, gpu, batch_size);

        }

        public void Dispose()
        {
            DisposeYolo();
        }

        static int[] gpu = { 0 };

        public static Iter_Data Get_IterationData()
        {
            try
            {
                return GetIterationData();
                //IntPtr p = GetIterationData();

                //if (p != null)
                //{
                //    Iter_Data h = (Iter_Data)Marshal.PtrToStructure(p, typeof(Iter_Data));
                //    p = new IntPtr(0);
                //    return h;
                //}
                //else
                //{
                //    Iter_Data tmp = new Iter_Data();
                //    tmp.max_batch = 0;
                //    return tmp;
                //}
            }
            catch
            {
                Iter_Data tmp = new Iter_Data();
                tmp.max_batch = 0;
                return tmp;
            }

        }

        public static void Train(object filesPath)
        {
            object[] param = (object[])filesPath;
            string dataPath = param[0].ToString();
            string cfgPath = param[1].ToString();
            string weightsPath = param[2].ToString();
            try
            {
                train(dataPath, cfgPath, null, 1, 0);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            //train(dataPath, cfgPath, weightsPath, 1, 0);
        }

        public bbox_t[] Detect(string filename)
        {

            var container = new BboxContainer();
            var count = DetectImage(filename, ref container);

            return container.candidates;
        }

        public bbox_t[] Detect(byte[] imageData)
        {
            var container = new BboxContainer();

            var size = Marshal.SizeOf(imageData[0]) * imageData.Length;
            var pnt = Marshal.AllocHGlobal(size);

            try
            {
                // Copy the array to unmanaged memory.
                Marshal.Copy(imageData, 0, pnt, imageData.Length);
                var count = DetectImage(pnt, imageData.Length, ref container);
                if (count == -1)
                {
                    throw new NotSupportedException($"{YoloLibraryName} has no OpenCV support");
                }
            }
            catch (Exception exception)
            {
                return null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }

            return container.candidates;
        }
    }

}

