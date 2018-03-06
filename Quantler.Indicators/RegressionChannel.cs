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

namespace Quantler.Indicators
{
    /// <summary>
    /// The Regression Channel indicator extends the <see cref="LeastSquaresMovingAverage"/>
    /// with the inclusion of two (upper and lower) channel lines that are distanced from
    /// the linear regression line by a user defined number of standard deviations.
    /// Reference: http://www.onlinetradingconcepts.com/TechnicalAnalysis/LinRegChannel.html
    /// </summary>
    public class RegressionChannel : Indicator
    {
        #region Private Fields

        /// <summary>
        /// Gets the standard deviation
        /// </summary>
        private IndicatorBase<IndicatorDataPoint> _standardDeviation;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionChannel"/> class.
        /// </summary>
        /// <param name="name">The name of this indicator</param>
        /// <param name="period">The number of data points to hold in the window</param>
        /// <param name="k">The number of standard deviations specifying the distance between the linear regression and upper or lower channel lines</param>
        public RegressionChannel(string name, int period, decimal k)
            : base(name)
        {
            _standardDeviation = new StandardDeviation(period);
            LinearRegression = new LeastSquaresMovingAverage(name + "_LinearRegression", period);
            LowerChannel = LinearRegression.Minus(_standardDeviation.Times(k), name + "_LowerChannel");
            UpperChannel = LinearRegression.Plus(_standardDeviation.Times(k), name + "_UpperChannel");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeastSquaresMovingAverage"/> class.
        /// </summary>
        /// <param name="period">The number of data points to hold in the window.</param>
        /// <param name="k">The number of standard deviations specifying the distance between the linear regression and upper or lower channel lines</param>
        public RegressionChannel(int period, decimal k)
            : this(string.Format("RC({0},{1})", period, k), period, k)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The point where the regression line crosses the y-axis (price-axis)
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> Intercept => LinearRegression.Intercept;

        /// <summary>
        /// Gets a flag indicating when this indicator is ready and fully initialized
        /// </summary>
        public override bool IsReady => _standardDeviation.IsReady && LinearRegression.IsReady && UpperChannel.IsReady && LowerChannel.IsReady;

        /// <summary>
        /// Gets the linear regression
        /// </summary>
        public LeastSquaresMovingAverage LinearRegression { get; }

        /// <summary>
        /// Gets the lower channel (linear regression - k * stdDev)
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> LowerChannel { get; }

        /// <summary>
        /// The regression line slope
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> Slope => LinearRegression.Slope;

        /// <summary>
        /// Gets the upper channel (linear regression + k * stdDev)
        /// </summary>
        public IndicatorBase<IndicatorDataPoint> UpperChannel { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Resets this indicator and all sub-indicators (StandardDeviation, LowerBand, MiddleBand, UpperBand)
        /// </summary>
        public override void Reset()
        {
            _standardDeviation.Reset();
            LinearRegression.Reset();
            LowerChannel.Reset();
            UpperChannel.Reset();
            base.Reset();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Computes the next value of this indicator from the given state
        /// </summary>
        /// <param name="input">The input given to the indicator</param>
        /// <returns>
        /// A new value for this indicator
        /// </returns>
        protected override decimal ComputeNextValue(IndicatorDataPoint input)
        {
            _standardDeviation.Update(input);
            LinearRegression.Update(input);
            LowerChannel.Update(input);
            UpperChannel.Update(input);

            return LinearRegression.Current;
        }

        #endregion Protected Methods
    }
}