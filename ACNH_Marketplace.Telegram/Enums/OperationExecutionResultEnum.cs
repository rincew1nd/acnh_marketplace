// <copyright file="OperationExecutionResultEnum.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Telegram.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Enum that represents state of executed operation.
    /// </summary>
    public enum OperationExecutionResult
    {
        /// <summary>
        /// Operation executed successfully
        /// </summary>
        Success,

        /// <summary>
        /// Operation failed with exception.
        /// </summary>
        Error,

        /// <summary>
        /// Need to reroute request.
        /// </summary>
        Reroute,
    }
}
