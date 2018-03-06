﻿#region License Header

/*
* QUANTLER.COM - Quant Fund Development Platform
* Quantler Core Trading Engine. Copyright 2018 Quantler B.V.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

#endregion License Header

namespace Quantler.Data.Bars
{
    /// <summary>
    /// Generic bar interface: Open, High, Low and Close.
    /// </summary>
    public interface Bar
    {
        #region Public Properties

        /// <summary>
        /// Closing price of the bar. Defined as the price at Start Time + TimeSpan.
        /// </summary>
        decimal Close { get; }

        /// <summary>
        /// High price of the bar during the time period.
        /// </summary>
        decimal High { get; }

        /// <summary>
        /// Low price of the bar during the time period.
        /// </summary>
        decimal Low { get; }

        /// <summary>
        /// Opening price of the bar: Defined as the price at the start of the time period.
        /// </summary>
        decimal Open { get; }

        #endregion Public Properties
    }
}