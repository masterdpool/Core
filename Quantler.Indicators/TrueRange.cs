﻿#region License Header

/*
* QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
* Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
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
*
* Modifications Copyright 2018 Quantler B.V.
*
*/

#endregion License Header

using Quantler.Data.Bars;
using System;

namespace Quantler.Indicators
{
    /// <summary>
    /// This indicator computes the True Range (TR).
    /// The True Range is the greatest of the following values:
    /// value1 = distance from today's high to today's low.
    /// value2 = distance from yesterday's close to today's high.
    /// value3 = distance from yesterday's close to today's low.
    /// </summary>
    public class TrueRange : BarIndicator
    {
        #region Private Fields

        private DataPointBar _previousInput;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TrueRange"/> class using the specified name.
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        public TrueRange(string name)
            : base(name)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady =>
            Samples > 1;

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>A new value for this indicator</returns>
        protected override decimal ComputeNextValue(DataPointBar input)
        {
            if (!IsReady)
            {
                _previousInput = input;
                return 0m;
            }

            var greatest = input.High - input.Low;

            var value2 = Math.Abs(_previousInput.Close - input.High);
            if (value2 > greatest)
                greatest = value2;

            var value3 = Math.Abs(_previousInput.Close - input.Low);
            if (value3 > greatest)
                greatest = value3;

            _previousInput = input;

            return greatest;
        }

        #endregion Protected Methods
    }
}