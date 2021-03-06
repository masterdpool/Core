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

using System.Collections.Generic;

namespace Quantler.Messaging
{
    /// <summary>
    /// Initialize new live trading instance
    /// </summary>
    public class LiveTradingMessage : MessageImpl
    {
        #region Public Properties

        /// <summary>
        /// Account identifier
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Type of account (cash or margin)
        /// </summary>
        public string AccountType { get; set; }

        /// <summary>
        /// Base currency known or to use (USD/EUR/JPY)
        /// </summary>
        public string BaseCurrency { get; set; }

        /// <summary>
        /// Broker model to use for live trading
        /// </summary>
        public string BrokerType { get; set; }

        /// <summary>
        /// Additional custom data that can be filled in
        /// </summary>
        public Dictionary<string, string> CustomData { get; set; }

        /// <summary>
        /// Display currency known or to use (USD/EUR/JPY)
        /// </summary>
        public string DisplayCurrency { get; set; }

        /// <summary>
        /// If true, make use of extended market hours
        /// </summary>
        public bool ExtendedMarketHours { get; set; }

        /// <summary>
        /// Gets or sets the leverage to use.
        /// </summary>
        public int Leverage { get; set; }

        /// <summary>
        /// Type of message
        /// </summary>
        public override MessageType MessageType { get; set; } = MessageType.LiveTrading;

        /// <summary>
        /// Associated portfolio id
        /// </summary>
        public string PortfolioId { get; set; }

        /// <summary>
        /// Gets or sets the initial quant fund.
        /// </summary>
        public AddFundMessage QuantFund { get; set; }

        #endregion Public Properties
    }
}