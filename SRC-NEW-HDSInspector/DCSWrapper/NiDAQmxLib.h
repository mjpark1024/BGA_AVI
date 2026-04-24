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

// NiDAQmxLib.h

#pragma once

#if  __ARCH__ == 64
	#include "../include/include64/NIDAQmx.h"
#else
	#include "../include/include32/NIDAQmx.h"
#endif
using namespace System;

namespace DCSWrapper {

	public ref class NiDAQmxWrapper
	{

	public :
				NiDAQmxWrapper();
				virtual ~NiDAQmxWrapper();

	public :
		/******************************************************/
		/***         Task Configuration/Control             ***/
		/******************************************************/
			
				int32 LoadTask                  (const char taskName[], TaskHandle *taskHandle);
				int32	CreateTask                (const char taskName[], TaskHandle *taskHandle);
				int32 AddGlobalChansToTask      (TaskHandle taskHandle, const char channelNames[]);
				
				int32 StartTask                 (TaskHandle taskHandle);
				int32 StopTask                  (TaskHandle taskHandle);
				
				int32 ClearTask                 (TaskHandle taskHandle);
				
				int32 WaitUntilTaskDone         (TaskHandle taskHandle, float64 timeToWait);
				int32 IsTaskDone                (TaskHandle taskHandle, bool32 *isTaskDone);
				
				int32 TaskControl               (TaskHandle taskHandle, int32 action);
				
				int32 GetNthTaskChannel         (TaskHandle taskHandle, uInt32 index, char buffer[], int32 bufferSize);
				
				int32 GetNthTaskDevice          (TaskHandle taskHandle, uInt32 index, char buffer[], int32 bufferSize);
				
				typedef int32 (CVICALLBACK *DAQmxEveryNSamplesEventCallbackPtr)(TaskHandle taskHandle, int32 everyNsamplesEventType, uInt32 nSamples, void *callbackData);
				typedef int32 (CVICALLBACK *DAQmxDoneEventCallbackPtr)(TaskHandle taskHandle, int32 status, void *callbackData);
				typedef int32 (CVICALLBACK *DAQmxSignalEventCallbackPtr)(TaskHandle taskHandle, int32 signalID, void *callbackData);
				

				int32 RegisterEveryNSamplesEvent(TaskHandle task, int32 everyNsamplesEventType, uInt32 nSamples, uInt32 options, DAQmxEveryNSamplesEventCallbackPtr callbackFunction, void *callbackData);
				int32 RegisterDoneEvent         (TaskHandle task, uInt32 options, DAQmxDoneEventCallbackPtr callbackFunction, void *callbackData);
				int32 RegisterSignalEvent       (TaskHandle task, int32 signalID, uInt32 options, DAQmxSignalEventCallbackPtr callbackFunction, void *callbackData);

				/******************************************************/
				/***                    Timing                      ***/
				/******************************************************/
				// (Analog/Counter Timing)
				int32      CfgSampClkTiming          (TaskHandle taskHandle, const char source[], float64 rate, int32 activeEdge, int32 sampleMode, uInt64 sampsPerChan);
				// (Digital Timing)
				int32      CfgHandshakingTiming      (TaskHandle taskHandle, int32 sampleMode, uInt64 sampsPerChan);
				// (Burst Import Clock Timing)
				int32      CfgBurstHandshakingTimingImportClock(TaskHandle taskHandle, int32 sampleMode, uInt64 sampsPerChan, float64 sampleClkRate, const char sampleClkSrc[], int32 sampleClkActiveEdge, int32 pauseWhen, int32 readyEventActiveLevel);
				// (Burst Export Clock Timing)
				int32      CfgBurstHandshakingTimingExportClock(TaskHandle taskHandle, int32 sampleMode, uInt64 sampsPerChan, float64 sampleClkRate, const char sampleClkOutpTerm[], int32 sampleClkPulsePolarity, int32 pauseWhen, int32 readyEventActiveLevel);
				int32      CfgChangeDetectionTiming  (TaskHandle taskHandle, const char risingEdgeChan[], const char fallingEdgeChan[], int32 sampleMode, uInt64 sampsPerChan);
				// (Counter Timing)
				int32      CfgImplicitTiming         (TaskHandle taskHandle, int32 sampleMode, uInt64 sampsPerChan);
				// (Pipelined Sample Clock Timing)
				int32      CfgPipelinedSampClkTiming (TaskHandle taskHandle, const char source[], float64 rate, int32 activeEdge, int32 sampleMode, uInt64 sampsPerChan);

				int32      ResetTimingAttribute      (TaskHandle taskHandle, int32 attribute);

				int32      ResetTimingAttributeEx    (TaskHandle taskHandle, const char deviceNames[], int32 attribute);



				/******************************************************/
				/***                  Triggering                    ***/
				/******************************************************/

				int32 DisableStartTrig          (TaskHandle taskHandle);
				int32 CfgDigEdgeStartTrig       (TaskHandle taskHandle, const char triggerSource[], int32 triggerEdge);
				int32 CfgAnlgEdgeStartTrig      (TaskHandle taskHandle, const char triggerSource[], int32 triggerSlope, float64 triggerLevel);
				int32 CfgAnlgWindowStartTrig    (TaskHandle taskHandle, const char triggerSource[], int32 triggerWhen, float64 windowTop, float64 windowBottom);
				int32 CfgDigPatternStartTrig    (TaskHandle taskHandle, const char triggerSource[], const char triggerPattern[], int32 triggerWhen);

				int32 DisableRefTrig            (TaskHandle taskHandle);
				int32 CfgDigEdgeRefTrig         (TaskHandle taskHandle, const char triggerSource[], int32 triggerEdge, uInt32 pretriggerSamples);
				int32 CfgAnlgEdgeRefTrig        (TaskHandle taskHandle, const char triggerSource[], int32 triggerSlope, float64 triggerLevel, uInt32 pretriggerSamples);
				int32 CfgAnlgWindowRefTrig      (TaskHandle taskHandle, const char triggerSource[], int32 triggerWhen, float64 windowTop, float64 windowBottom, uInt32 pretriggerSamples);
				int32 CfgDigPatternRefTrig      (TaskHandle taskHandle, const char triggerSource[], const char triggerPattern[], int32 triggerWhen, uInt32 pretriggerSamples);

				int32 DisableAdvTrig            (TaskHandle taskHandle);
				int32 CfgDigEdgeAdvTrig         (TaskHandle taskHandle, const char triggerSource[], int32 triggerEdge);

				int32 ResetTrigAttribute        (TaskHandle taskHandle, int32 attribute);

				int32 SendSoftwareTrigger       (TaskHandle taskHandle, int32 triggerID);

				/******************************************************/
				/***        Channel Configuration/Creation          ***/
				/******************************************************/
				int32 CreateCOPulseChanFreq     (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 units, int32 idleState, float64 initialDelay, float64 freq, float64 dutyCycle);
				int32 CreateCOPulseChanTime     (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 units, int32 idleState, float64 initialDelay, float64 lowTime, float64 highTime);
				int32 CreateCOPulseChanTicks    (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], const char sourceTerminal[], int32 idleState, int32 initialDelay, int32 lowTicks, int32 highTicks);

				int32 CreateCIPulseChanTicks    (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], const char sourceTerminal[], float64 minVal, float64 maxVal);
				int32 SetCIPulseFreqDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data);

				//time

				int32 SetCOCtrTimebaseDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data);
				int32 SetCOCtrTimebaseDigFltrMinPulseWidth(TaskHandle taskHandle, const char channel[], float64 data);


				//Count Edge
				int32 CreateCICountEdgesChan (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 edge, uInt32 initialCount, int32 countDirection);
				int32 SetCICountEdgesDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data);
				int32 SetCICountEdgesDigFltrMinPulseWidth(TaskHandle taskHandle, const char channel[], float64 data);
				//주기/주파수
				int32 CreateCIFreqChan (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], float64 minVal, float64 maxVal, int32 units, int32 edge, int32 measMethod, float64 measTime, uInt32 divisor, const char customScaleName[]);
				int32 SetCIFreqDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data);

				//위치측정
				int32 CreateCILinEncoderChan (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 decodingType, bool32 ZidxEnable, float64 ZidxVal, int32 ZidxPhase, int32 units, float64 distPerPulse, float64 initialPos, const char customScaleName[]);
				int32 CreateCIAngEncoderChan (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], int32 decodingType, bool32 ZidxEnable, float64 ZidxVal, int32 ZidxPhase, int32 units, uInt32 pulsesPerRev, float64 initialAngle, const char customScaleName[]);


				int32 SetCIEncoderAInputDigFltrEnable(TaskHandle taskHandle, const char channel[], bool32 data);
				int32 SetCIEncoderAInputDigFltrMinPulseWidth(TaskHandle taskHandle, const char channel[], float64 data);

				int32 WriteCtrTicks (TaskHandle taskHandle, int32 numSampsPerChan, bool32 autoStart, float64 timeout, bool32 dataLayout, uInt32 highTicks[], uInt32 lowTicks[], int32 *numSampsPerChanWritten, bool32 *reserved);
				//int32 SetStartTrigRetriggerable(TaskHandle taskHandle, bool32 data);

					/******************************************************/
				/***                 Error Handling                 ***/
				/******************************************************/
				int32  GetErrorString            (int32 errorCode, char errorString[], uInt32 bufferSize);
				int32 GetExtendedErrorInfo      (char errorString[], uInt32 bufferSize);


				/******************************************************/
				/***        Channel Configuration/Creation          ***/
				/******************************************************/
				int32 CreateCIPulseChanFreq     (TaskHandle taskHandle, const char counter[], const char nameToAssignToChannel[], float64 minVal, float64 maxVal, int32 units);
				int32 GetCIMeasType(TaskHandle taskHandle, const char channel[], int32 *data);

				/***************************************************/
				/******           Digital IN/OUT				****/
				/***************************************************/
				int32 CreateDIChan(TaskHandle taskHandle, const char lines[], const char nameToAssignToLines[], int32 lineGrouping);
				int32 CreateDOChan(TaskHandle taskHandle, const char lines[], const char nameToAssignToLines[], int32 lineGrouping);
				int32 ReadDigitalLines(TaskHandle taskHandle, int32 numSampsPerChan, float64 timeout, bool32 fillMode, uInt8 readArray[], uInt32 arraySizeInBytes, int32 *sampsPerChanRead, int32 *numBytesPerSamp, bool32 *reserved);
				int32 WriteDigitalLines(TaskHandle taskHandle, int32 numSampsPerChan, bool32 autoStart, float64 timeout, bool32 dataLayout, uInt8 writeArray[], int32 *sampsPerChanWritten, bool32 *reserved);
				int32 WriteDigitalU32 (TaskHandle taskHandle, int32 numSampsPerChan, bool32 autoStart, float64 timeout, bool32 dataLayout, uInt32 writeArray[], int32 *sampsPerChanWritten, bool32 *reserved);
		        int32 ReadDigitalU32 (TaskHandle taskHandle, int32 numSampsPerChan, float64 timeout, bool32 fillMode, uInt32 readArray[], uInt32 arraySizeInSamps, int32 *sampsPerChanRead, bool32 *reserved);
	};
}
