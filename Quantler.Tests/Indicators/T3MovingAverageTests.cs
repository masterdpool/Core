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

using FluentAssertions;
using Quantler.Indicators;
using Xunit;

namespace Quantler.Tests.Indicators
{
    public class T3MovingAverageTests
    {
        #region Public Methods

        [Fact]
        [Trait("Quantler.Indicators", "T3MovingAverage")]
        public void ComparesAgainstExternalData()
        {
            var indicator = new T3MovingAverage(5);

            RunTestIndicator(indicator);
        }

        [Fact]
        [Trait("Quantler.Indicators", "T3MovingAverage")]
        public void ComparesAgainstExternalDataAfterReset()
        {
            var indicator = new T3MovingAverage(5);

            RunTestIndicator(indicator);
            indicator.Reset();
            RunTestIndicator(indicator);
        }

        [Fact]
        [Trait("Quantler.Indicators", "T3MovingAverage")]
        public void ResetsProperly()
        {
            var indicator = new T3MovingAverage(5, 1);

            TestHelper.TestIndicatorReset(indicator, "spy_t3.txt");
        }

        #endregion Public Methods

        #region Private Methods

        private static void RunTestIndicator(IndicatorBase<IndicatorDataPoint> indicator)
        {
            TestHelper.TestIndicator(indicator, "spy_t3.txt", "T3_5", (ind, expected) =>
                ((double) ind.Current.Price).Should().BeApproximately(expected, 2e-2));
        }

        #endregion Private Methods
    }
}