﻿#region License Header
/*
*
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
*
*/
#endregion License Header

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using Quantler.Exchanges;
using Quantler.Interfaces;
using Quantler.Modules;
using Quantler.Orders;
using Quantler.Securities;

namespace Quantler.Local.Modules.MoneyManagement
{
    /// <summary>
    /// Periodically rebalances the current universe per x days
    /// </summary>
    /// <seealso cref="MoneyManagementModule" />
    [Export(typeof(IModule))]
    public class PeriodicallyRebalance : MoneyManagementModule
    {
        //Rebalance every x calendar days (default is every 2 weeks)
        [Parameter(10, 365, 1, "Days")]
        public int RebalanceDays { get; set; } = 14;

        //Amount of hours to wait before executing the rebalance
        [Parameter(0, 23, 1, "Hours")]
        public int HoursWait { get; set; } = 1;

        //Keep track of last rebalance
        readonly Dictionary<string, DateTime> _lastRebalance = new Dictionary<string, DateTime>();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            //Check for each exchange on the start of every trading day if we need to rebalance
            foreach (var exchange in Universe.Exchanges)
            {
                //Set schedule to check each start of the trading day (HoursWait after market opens)
                Schedule(Date.EveryTradingDay(exchange), Time.AfterMarketOpen(exchange, TimeSpan.FromHours(HoursWait)),
                    () => Rebalance(Universe.GetSecurities(x => x.Exchange == exchange), exchange));

                //Check for each exchange
                _lastRebalance.Add(exchange.Name, DateTime.MinValue);
            }
        }

        /// <summary>
        /// Calculate order size for signal order
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="state"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public override decimal OrderQuantity(SubmitOrderTicket ticket, SecurityState state, decimal weight)
        {
            //Get target amount
            decimal target = TargetWeight(ticket.Security,
                  (state == SecurityState.EntryShort ? -1 : state == SecurityState.EntryLong ? 1 : 0) * weight);

            //Check if target is valid
            if (target == 0)
            {
                ticket.Cancel();
                return 0;
            }
            else
                return target;
        }

        /// <summary>
        /// Rebalances the specified securities.
        /// </summary>
        /// <param name="securities">The securities.</param>
        /// <param name="exchange">Associated exchange information</param>
        private void Rebalance(Security[] securities, ExchangeModel exchange)
        {
            //Check if we have securities
            if (securities.Length == 0)
                return;

            //Check if we are supposed to rebalance based on the parameter supplied
            DateTime utcnow = exchange.UtcTime.Date;
            if (utcnow < _lastRebalance[exchange.Name].Date.AddDays(RebalanceDays))
                return;

            //Rebalance (using market orders)
            foreach (var security in securities.Where(x => !Position[x].IsFlat))
            {
                //We can send order size zero as the order quantity method process the quantity anyways
                SubmitOrderTicket(MarketOnOpenOrder(security, 0m, "Rebalancing"));
                Info($"Rebalanced security {security.Ticker.Name} for exchange {exchange.Name} on local date and time {exchange.LocalTime}");
            }

            //Set last rebalance
            _lastRebalance[exchange.Name] = utcnow;
        }
    }
}
