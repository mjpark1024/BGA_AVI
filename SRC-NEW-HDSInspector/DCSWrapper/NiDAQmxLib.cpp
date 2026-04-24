/*============================================================================*/
/*                 National Instruments / Data Acquisition                    */
/*----------------------------------------------------------------------------*/
/*    Copyright (c) National Instruments 2003.  All Rights Reserved.          */
/*----------------------------------------------------------------------------*/
/*                                                                            */
/* Title:       NIDAQmx.h                                                     */
/* Purpose:     Include file for NI-DAQmx library support.                    */
/*                                                                            */
/*============================================================================*/

#include "stdafx.h"

#include "NiDAQmxLib.h"

namespace DCSWrapper
{

        NiDAQmxWrapper::NiDAQmxWrapper()
        {
		}

        NiDAQmxWrapper::~NiDAQmxWrapper()
        {
        }

        int32  NiDAQmxWrapper::LoadTask( const char taskName[], TaskHandle *taskHandle )
        {
               return DAQmxLoadTask( taskName, taskHandle );
        }

		 int32  NiDAQmxWrapper::CreateTask( const char taskName[], TaskHandle *taskHandle )
        {
               return DAQmxCreateTask( taskName, taskHandle );
        }

		 int32  NiDAQmxWrapper::AddGlobalChansToTask (TaskHandle taskHandle, const char channelNames[])
        {
               return DAQmxAddGlobalChansToTask(taskHandle, channelNames);
        }

		int32  NiDAQmxWrapper::StartTask( TaskHandle taskHandle )
        {
               return DAQmxStartTask( taskHandle );
        }

		int32  NiDAQmxWrapper::StopTask( TaskHandle taskHandle )
        {
               return DAQmxStopTask( taskHandle );
        }

		int32  NiDAQmxWrapper::ClearTask( TaskHandle taskHandle )
        {
               return DAQmxClearTask( taskHandle );
        }

		int32  NiDAQmxWrapper::WaitUntilTaskDone( TaskHandle taskHandle, float64 timeToWait )
        {
               return DAQmxWaitUntilTaskDone( taskHandle, timeToWait );
        }

		int32  NiDAQmxWrapper::IsTaskDone(TaskHandle taskHandle, bool32 *isTaskDone)
        {
               return DAQmxIsTaskDone( taskHandle, isTaskDone );
        }

		int32  NiDAQmxWrapper::TaskControl(TaskHandle taskHandle, int32 action)
        {
             return DAQmxTaskControl( taskHandle, action );
        }

		int32  NiDAQmxWrapper::GetNthTaskChannel(TaskHandle taskHandle, uInt32 index, char buffer[], int32 bufferSize)
        {
             return DAQmxGetNthTaskChannel(taskHandle, index, buffer, bufferSize );
        }

		int32  NiDAQmxWrapper::GetNthTaskDevice(TaskHandle taskHandle, uInt32 index, char buffer[], int32 bufferSize)
        {
             return DAQmxGetNthTaskDevice( taskHandle, index, buffer, bufferSize );
        }

		int32 NiDAQmxWrapper::RegisterEveryNSamplesEvent(TaskHandle task, int32 everyNsamplesEventType, uInt32 nSamples, uInt32 options, DAQmxEveryNSamplesEventCallbackPtr callbackFunction, void *callbackData)
		{
			return DAQmxRegisterEveryNSamplesEvent(task, everyNsamplesEventType,  nSamples, options,  callbackFunction, callbackData);
		}
		int32 NiDAQmxWrapper::RegisterDoneEvent(TaskHandle task, uInt32 options, DAQmxDoneEventCallbackPtr callbackFunction, void *callbackData)
		{
			return DAQmxRegisterDoneEvent(task, options, callbackFunction, callbackData);
		}
		int32 NiDAQmxWrapper::RegisterSignalEvent(TaskHandle task, int32 signalID, uInt32 options, DAQmxSignalEventCallbackPtr callbackFunction, void *callbackData)
		{
			return DAQmxRegisterSignalEvent(task, signalID,options, callbackFunction, callbackData);
		}

		
		int32 NiDAQmxWrapper::DisableStartTrig          (TaskHandle taskHandle)
		{
			return DAQmxDisableStartTrig(taskHandle);
		}
		int32 NiDAQmxWrapper::CfgDigEdgeStartTrig       (TaskHandle taskHandle, const char triggerSource[], int32 triggerEdge)
		{
			return DAQmxCfgDigEdgeStartTrig(taskHandle, triggerSource, triggerEdge);
		}
		int32 NiDAQmxWrapper::CfgAnlgEdgeStartTrig      (TaskHandle taskHandle, const char triggerSource[], int32 triggerSlope, float64 triggerLevel)
		{
			return DAQmxCfgAnlgEdgeStartTrig ( taskHandle, triggerSource, triggerSlope, triggerLevel);
		}
		int32 NiDAQmxWrapper::CfgAnlgWindowStartTrig    (TaskHandle taskHandle, const char triggerSource[], int32 triggerWhen, float64 windowTop, float64 windowBottom)
		{
			return DAQmxCfgAnlgWindowStartTrig(taskHandle, triggerSource, triggerWhen, windowTop, windowBottom);
		}
		int32 NiDAQmxWrapper::CfgDigPatternStartTrig    (TaskHandle taskHandle, const char triggerSource[], const char triggerPattern[], int32 triggerWhen)
		{
			return DAQmxCfgDigPatternStartTrig(taskHandle, triggerSource, triggerPattern, triggerWhen);
		}

		int32 NiDAQmxWrapper::DisableRefTrig            (TaskHandle taskHandle)
		{
			return DAQmxDisableRefTrig( taskHandle);
		}
		int32 NiDAQmxWrapper::CfgDigEdgeRefTrig         (TaskHandle taskHandle, const char triggerSource[], int32 triggerEdge, uInt32 pretriggerSamples)
		{
			return DAQmxCfgDigEdgeRefTrig(taskHandle, triggerSource, triggerEdge, pretriggerSamples);
		}
		int32 NiDAQmxWrapper::CfgAnlgEdgeRefTrig        (TaskHandle taskHandle, const char triggerSource[], int32 triggerSlope, float64 triggerLevel, uInt32 pretriggerSamples)
		{
			return DAQmxCfgAnlgEdgeRefTrig      (taskHandle,triggerSource, triggerSlope, triggerLevel, pretriggerSamples);
		}



		int32 NiDAQmxWrapper::CfgAnlgWindowRefTrig      (TaskHandle taskHandle, const char triggerSource[], int32 triggerWhen, float64 windowTop, float64 windowBottom, uInt32 pretriggerSamples)
		{
			return DAQmxCfgAnlgWindowRefTrig      (taskHandle,  triggerSource,  triggerWhen,  windowTop, windowBottom, pretriggerSamples);
		}
		int32 NiDAQmxWrapper::CfgDigPatternRefTrig      (TaskHandle taskHandle, const char triggerSource[], const char triggerPattern[], int32 triggerWhen, uInt32 pretriggerSamples)
		{
			return  DAQmxCfgDigPatternRefTrig( taskHandle, triggerSource, triggerPattern,triggerWhen, pretriggerSamples);
		}

		int32 NiDAQmxWrapper::DisableAdvTrig            (TaskHandle taskHandle)
		{
			return DAQmxDisableAdvTrig            (taskHandle);
		}
		int32 NiDAQmxWrapper::CfgDigEdgeAdvTrig         (TaskHandle taskHandle, const char triggerSource[], int32 triggerEdge)
		{
			return DAQmxCfgDigEdgeAdvTrig         ( taskHandle,triggerSource, triggerEdge);
		}

		int32 NiDAQmxWrapper::ResetTrigAttribute        (TaskHandle taskHandle, int32 attribute)
		{
			return DAQmxResetTrigAttribute        (taskHandle, attribute);
		}

		int32 NiDAQmxWrapper::SendSoftwareTrigger       (TaskHandle taskHandle, int32 triggerID)
		{
			return DAQmxSendSoftwareTrigger       (taskHandle, triggerID);
		}

		int32  NiDAQmxWrapper::GetErrorString            (int32 errorCode, char errorString[], uInt32 bufferSize)
		{
			return DAQmxGetErrorString            (errorCode, errorString, bufferSize);
		}
		int32 NiDAQmxWrapper::GetExtendedErrorInfo      (char errorString[], uInt32 bufferSize)
		{
			return DAQmxGetExtendedErrorInfo      (errorString, bufferSize);
		}
		
			// (Analog/Counter Timing)
		int32      NiDAQmxWrapper::CfgSampClkTiming          (TaskHandle taskHandle, const char source[], float64 rate, int32 activeEdge, int32 sampleMode, uInt64 sampsPerChan)
		{
			return DAQmxCfgSampClkTiming          ( taskHandle, source,  rate, activeEdge, sampleMode, sampsPerChan);
		}


		// (Digital Timing)
		int32     NiDAQmxWrapper:: CfgHandshakingTiming      (TaskHandle taskHandle, int32 sampleMode, uInt64 sampsPerChan)
		{
			return DAQmxCfgHandshakingTiming      (taskHandle, sampleMode, sampsPerChan);

		}

		// (Burst Import Clock Timing)
		int32      NiDAQmxWrapper::CfgBurstHandshakingTimingImportClock(TaskHandle taskHandle, int32 sampleMode, uInt64 sampsPerChan, float64 sampleClkRate, const char sampleClkSrc[], int32 sampleClkActiveEdge, int32 pauseWhen, int32 readyEventActiveLevel)
		{
			return DAQmxCfgBurstHandshakingTimingImportClock(taskHandle, sampleMode, sampsPerChan, sampleClkRate, sampleClkSrc, sampleClkActiveEdge,  pauseWhen, readyEventActiveLevel);
		}
		// (Burst Export Clock Timing)
		int32      NiDAQmxWrapper::CfgBurstHandshakingTimingExportClock(TaskHandle taskHandle, int32 sampleMode, uInt64 sampsPerChan, float64 sampleClkRate, const char sampleClkOutpTerm[], int32 sampleClkPulsePolarity, int32 pauseWhen, int32 readyEventActiveLevel)
		{
			return DAQmxCfgBurstHandshakingTimingExportClock(taskHandle, sampleMode, sampsPerChan, sampleClkRate, sampleClkOutpTerm, sampleClkPulsePolarity, pauseWhen, readyEventActiveLevel);
		}

		int32      NiDAQmxWrapper::CfgChangeDetectionTiming  (TaskHandle taskHandle, const char risingEdgeChan[], const char fallingEdgeChan[], int32 sampleMode, uInt64 sampsPerChan)
		{
			return DAQmxCfgChangeDetectionTiming  (taskHandle, risingEdgeChan, fallingEdgeChan,  sampleMode,  sampsPerChan);
		}
		// (Counter Timing)
		int32      NiDAQmxWrapper::CfgImplicitTiming         (TaskHandle taskHandle, int32 sampleMode, uInt64 sampsPerChan)
		{
			return DAQmxCfgImplicitTiming         ( taskHandle, sampleMode, sampsPerChan);
		}

		// (Pipelined Sample Clock Timing)
		int32      NiDAQmxWrapper::CfgPipelinedSampClkTiming (TaskHandle taskHandle, const char source[], float64 rate, int32 activeEdge, int32 sampleMode, uInt64 sampsPerChan)
		{
			return DAQmxCfgPipelinedSampClkTiming (taskHandle, source, rate, activeEdge, sampleMode, sampsPerChan);
		}

		int32      NiDAQmxWrapper::ResetTimingAttribute      (TaskHandle taskHandle, int32 attribute)
		{
			return DAQmxResetTimingAttribute      (taskHandle,  attribute);
		}

		int32      NiDAQmxWrapper::ResetTimingAttributeEx    (TaskHandle taskHandle, const char deviceNames[], int32 attribute)
		{
			return DAQmxResetTimingAttributeEx    (taskHandle, deviceNames, attribute);
		}


		int32 NiDAQmxWrapper::CreateCOPulseChanFreq     (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 units, int32 idleState, float64 initialDelay, float64 freq, float64 dutyCycle)
		{
			return DAQmxCreateCOPulseChanFreq (taskHandle,counter, nameToAssignToChannel, units,  idleState, initialDelay,  freq,  dutyCycle);
		}
		int32 NiDAQmxWrapper::CreateCOPulseChanTime     (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 units, int32 idleState, float64 initialDelay, float64 lowTime, float64 highTime)
		{
			return DAQmxCreateCOPulseChanTime     (taskHandle, counter,  nameToAssignToChannel, units, idleState, initialDelay,  lowTime,  highTime);
		}
		int32 NiDAQmxWrapper::CreateCOPulseChanTicks    (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], const char sourceTerminal[], int32 idleState, int32 initialDelay, int32 lowTicks, int32 highTicks)
		{
			return DAQmxCreateCOPulseChanTicks    (taskHandle, counter,  nameToAssignToChannel, sourceTerminal, idleState, initialDelay, lowTicks, highTicks);
		}

		int32 NiDAQmxWrapper::CreateCIPulseChanFreq    (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], float64 minVal, float64 maxVal, int32 units)
		{
			return DAQmxCreateCIPulseChanFreq (taskHandle,counter, nameToAssignToChannel,  minVal, maxVal, units);

		}
		// pulse chan
		int32 NiDAQmxWrapper::CreateCIPulseChanTicks    (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], const char sourceTerminal[], float64 minVal, float64 maxVal)

		{
			return DAQmxCreateCIPulseChanTicks    ( taskHandle, counter, nameToAssignToChannel,  sourceTerminal,  minVal,  maxVal);
		}

		int32 NiDAQmxWrapper::SetCIPulseFreqDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data)
		{
			return DAQmxSetCIPulseFreqDigFltrEnable(taskHandle, channel,  data);
		}


	
		int32 NiDAQmxWrapper::SetCOCtrTimebaseDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data)
		{
			return DAQmxSetCOCtrTimebaseDigFltrEnable(taskHandle, channel,  data);
		}
		int32 NiDAQmxWrapper::SetCOCtrTimebaseDigFltrMinPulseWidth(TaskHandle taskHandle, const char channel[], float64 data)
		{
			return DAQmxSetCOCtrTimebaseDigFltrMinPulseWidth( taskHandle,  channel, data);
		}

		// 입력 에지 카운트CreateCICountEdgesChan (taskHandle,  counter,  nameToAssignToChannel, Val_Falling, 0, Val_CountUp = 10128)
		int32 NiDAQmxWrapper::CreateCICountEdgesChan (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 edge, uInt32 initialCount, int32 countDirection)
		{
			return DAQmxCreateCICountEdgesChan (taskHandle, counter,  nameToAssignToChannel, edge,  initialCount,  countDirection);
		}

		int32 NiDAQmxWrapper::SetCICountEdgesDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data)
		{
			return DAQmxSetCICountEdgesDigFltrEnable(taskHandle, channel,  data);
		}
		int32 NiDAQmxWrapper::SetCICountEdgesDigFltrMinPulseWidth(TaskHandle taskHandle, const char channel[], float64 data)
		{
			return DAQmxSetCICountEdgesDigFltrMinPulseWidth( taskHandle,  channel, data);
		}

		int32 NiDAQmxWrapper::WriteCtrTicks (TaskHandle taskHandle, int32 numSampsPerChan, bool32 autoStart, float64 timeout, bool32 dataLayout, uInt32 highTicks[], uInt32 lowTicks[], int32 *numSampsPerChanWritten, bool32 *reserved)
		{
			return DAQmxWriteCtrTicks (taskHandle, numSampsPerChan,  autoStart, timeout, dataLayout, highTicks,  lowTicks, numSampsPerChanWritten, reserved);
		}

		// 주기/주파스 측정(높은 주파수)CreateCIFreqChan (TaskHandle taskHandle,  counter,  nameToAssignToChannel, 0,  8,  Val_Hz,  Val_Falling,  Val_LowFreq1Ctr = 10105,  measTime 10000,  0,  null)
		int32 NiDAQmxWrapper::CreateCIFreqChan (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], float64 minVal, float64 maxVal, int32 units, int32 edge, int32 measMethod, float64 measTime, uInt32 divisor, const char customScaleName[])
		{
			return DAQmxCreateCIFreqChan(taskHandle, counter, nameToAssignToChannel, minVal, maxVal, units, edge, measMethod, measTime, divisor, customScaleName);
		}

		int32 NiDAQmxWrapper::SetCIFreqDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data)
		{
			return DAQmxSetCIFreqDigFltrEnable(taskHandle, channel,  data);
		}
		
		// 위치 측정
		int32 NiDAQmxWrapper::CreateCIAngEncoderChan (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 decodingType, bool32 ZidxEnable, float64 ZidxVal, int32 ZidxPhase, int32 units, uInt32 pulsesPerRev, float64 initialAngle, const char customScaleName[])
		{
			return DAQmxCreateCIAngEncoderChan ( taskHandle, counter,  nameToAssignToChannel,  decodingType, ZidxEnable,  ZidxVal,  ZidxPhase,  units, pulsesPerRev,  initialAngle, customScaleName);
		}

		int32 NiDAQmxWrapper::CreateCILinEncoderChan (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 decodingType, bool32 ZidxEnable, float64 ZidxVal, int32 ZidxPhase, int32 units, float64 distPerPulse, float64 initialPos, const char customScaleName[])
		{
			return DAQmxCreateCILinEncoderChan (taskHandle,counter, nameToAssignToChannel, decodingType, ZidxEnable, ZidxVal, ZidxPhase, units, distPerPulse, initialPos, customScaleName);

		}
		int32 NiDAQmxWrapper::SetCIEncoderAInputDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data)
		{
			return DAQmxSetCIEncoderAInputDigFltrEnable( taskHandle,  channel, data);
		}
		
		int32 NiDAQmxWrapper::SetCIEncoderAInputDigFltrMinPulseWidth(TaskHandle taskHandle, const char channel[], float64 data)
		{
			return DAQmxSetCIEncoderAInputDigFltrMinPulseWidth( taskHandle,  channel, data);
		}




		int32 NiDAQmxWrapper::GetCIMeasType(TaskHandle taskHandle, const char channel[], int32 *data)
		{
			return DAQmxGetCIMeasType (taskHandle,channel,  data);
		}


		int32 NiDAQmxWrapper::CreateDIChan(TaskHandle taskHandle, const char lines[], const char nameToAssignToLines[], int32 lineGrouping)
		{
			return DAQmxCreateDIChan(taskHandle, lines, nameToAssignToLines, lineGrouping);
		}

		int32 NiDAQmxWrapper::CreateDOChan(TaskHandle taskHandle, const char lines[], const char nameToAssignToLines[], int32 lineGrouping)
		{
			return DAQmxCreateDOChan(taskHandle, lines, nameToAssignToLines, lineGrouping);
		}

		int32 NiDAQmxWrapper::ReadDigitalLines(TaskHandle taskHandle, int32 numSampsPerChan, float64 timeout, bool32 fillMode, uInt8 readArray[], uInt32 arraySizeInBytes, int32 *sampsPerChanRead, int32 *numBytesPerSamp, bool32 *reserved)
		{
			return DAQmxReadDigitalLines( taskHandle,  numSampsPerChan,  timeout, fillMode, readArray, arraySizeInBytes, sampsPerChanRead, numBytesPerSamp, reserved);
		}

		int32 NiDAQmxWrapper::WriteDigitalLines(TaskHandle taskHandle, int32 numSampsPerChan, bool32 autoStart, float64 timeout, bool32 dataLayout, uInt8 writeArray[], int32 *sampsPerChanWritten, bool32 *reserved)
		{
			return DAQmxWriteDigitalLines( taskHandle, numSampsPerChan,  autoStart,  timeout,  dataLayout, writeArray, sampsPerChanWritten,reserved);
		}

		int32 NiDAQmxWrapper::WriteDigitalU32 (TaskHandle taskHandle, int32 numSampsPerChan, bool32 autoStart, float64 timeout, bool32 dataLayout, uInt32 writeArray[], int32 *sampsPerChanWritten, bool32 *reserved)
		{
			return DAQmxWriteDigitalU32 (taskHandle, numSampsPerChan, autoStart, timeout, dataLayout, writeArray, sampsPerChanWritten, reserved);
		}

		int32 NiDAQmxWrapper::ReadDigitalU32 (TaskHandle taskHandle, int32 numSampsPerChan, float64 timeout, bool32 fillMode, uInt32 readArray[], uInt32 arraySizeInSamps, int32 *sampsPerChanRead, bool32 *reserved)
		{
			return DAQmxReadDigitalU32 (taskHandle, numSampsPerChan, timeout, fillMode, readArray, arraySizeInSamps, sampsPerChanRead, reserved);
		}


}

 
