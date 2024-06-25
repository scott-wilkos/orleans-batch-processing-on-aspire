﻿namespace BatchProcessing.ApiService.Grains;

public interface IEngineGrain : IGrainWithGuidKey
{
    Task RunAnalysis(int recordsToSimulate);

    Task<EngineStatusRecord> GetStatus();
}