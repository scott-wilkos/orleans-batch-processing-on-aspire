﻿namespace BatchProcessing.Domain;

public enum BatchProcessStatusEnum
{
    Created = 0,
    Running = 1,
    Completed = 2,
    Error = 3
}