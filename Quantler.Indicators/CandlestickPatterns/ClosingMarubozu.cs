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

namespace Quantler.Indicators.CandlestickPatterns
{
    /// <summary>
    /// Closing Marubozu candlestick pattern indicator
    /// </summary>
    /// <remarks>
    /// Must have:
    /// - long white(black) real body
    /// - no or very short upper(lower) shadow
    /// The meaning of "long" and "very short" is specified with SetCandleSettings
    /// The returned value is positive(+1) when white(bullish), negative(-1) when black(bearish)
    /// </remarks>
    public class ClosingMarubozu : CandlestickPattern
    {
        #region Private Fields

        private readonly int _bodyLongAveragePeriod;
        private readonly int _shadowVeryShortAveragePeriod;

        private decimal _bodyLongPeriodTotal;
        private decimal _shadowVeryShortPeriodTotal;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingMarubozu"/> class using the specified name.
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        public ClosingMarubozu(string name)
            : base(name, Math.Max(CandleSettings.Get(CandleSettingType.BodyLong).AveragePeriod, CandleSettings.Get(CandleSettingType.ShadowVeryShort).AveragePeriod) + 1)
        {
            _bodyLongAveragePeriod = CandleSettings.Get(CandleSettingType.BodyLong).AveragePeriod;
            _shadowVeryShortAveragePeriod = CandleSettings.Get(CandleSettingType.ShadowVeryShort).AveragePeriod;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingMarubozu"/> class.
        /// </summary>
        public ClosingMarubozu()
            : this("CLOSINGMARUBOZU")
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady =>
            Samples >= Period;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resets this indicator to its initial state
        /// </summary>
        public override void Reset()
        {
            _bodyLongPeriodTotal = 0m;
            _shadowVeryShortPeriodTotal = 0m;
            base.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="window">The window of data held in this indicator</param>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>A new value for this indicator</returns>
        protected override decimal ComputeNextValue(IReadOnlyWindow<DataPointBar> window, DataPointBar input)
        {
            if (!IsReady)
            {
                if (Samples >= Period - _bodyLongAveragePeriod)
                {
                    _bodyLongPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, input);
                }

                if (Samples >= Period - _shadowVeryShortAveragePeriod)
                {
                    _shadowVeryShortPeriodTotal += GetCandleRange(CandleSettingType.ShadowVeryShort, input);
                }

                return 0m;
            }

            decimal value;
            if (
                // long body
                GetRealBody(input) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal, input) &&
                (
                  (
                    // white body and very short upper shadow
                    GetCandleColor(input) == CandleColor.White &&
                    GetUpperShadow(input) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal, input)
                  ) ||
                  (
                    // black body and very short lower shadow
                    GetCandleColor(input) == CandleColor.Black &&
                    GetLowerShadow(input) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal, input)
                  )
                ))
                value = (int)GetCandleColor(input);
            else
                value = 0m;

            // add the current range and subtract the first range: this is done after the pattern recognition
            // when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)

            _bodyLongPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, input) -
                                    GetCandleRange(CandleSettingType.BodyLong, window[_bodyLongAveragePeriod]);

            _shadowVeryShortPeriodTotal += GetCandleRange(CandleSettingType.ShadowVeryShort, input) -
                                           GetCandleRange(CandleSettingType.ShadowVeryShort, window[_shadowVeryShortAveragePeriod]);

            return value;
        }

        #endregion Protected Methods
    }
}