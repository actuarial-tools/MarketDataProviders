﻿/* Copyright (C) 2013 Fairmat SRL (info@fairmat.com, http://www.fairmat.com/)
 * Author(s): Stefano Angeleri (stefano.angeleri@fairmat.com)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using DVPLI;
using DVPLI.MarketDataTypes;
using NUnit.Framework;

namespace MarketDataProviders.Tests.MEFFIntegration
{
    /// <summary>
    /// Tests the Fairmat interface to MEFF API provided by
    /// <see cref="MEFFIntegration"/>.
    /// </summary>
    [TestFixture]
    [Ignore("MEEF doesn't have these things anymore")]
    public class TestMEFFIntegration
    {
        /// <summary>
        /// Initializes the backend to run the tests.
        /// </summary>
        [SetUp]
        public void Init()
        {
            TestCommon.TestInitialization.CommonInitialization();
        }

        /// <summary>
        /// Tests the TestConnectivity() method of the
        /// interface and checks if it works without errors.
        /// </summary>
        [Test]
        public void TestConnectivity()
        {
            global::MEFFIntegration.MEFFIntegration wrapper = new global::MEFFIntegration.MEFFIntegration();
            Status status = wrapper.TestConnectivity();
            Assert.That(!status.HasErrors, status.ErrorMessage);
        }

        /// <summary>
        /// Tests requesting a single entry and checks values correspond.
        /// The values of the ticker are approximate.
        /// </summary>
        [Test]
        public void TestRequestOneEntry()
        {
            global::MEFFIntegration.MEFFIntegration wrapper = new global::MEFFIntegration.MEFFIntegration();
            IMarketData data;
            MarketDataQuery query = new MarketDataQuery();
            query.Ticker = "GRF";
            query.Date = new DateTime(2013, 6, 3);
            query.MarketDataType = typeof(Scalar).ToString();
            query.Field = "close";

            Status status = wrapper.GetMarketData(query, out data);

            Assert.That(!status.HasErrors, status.ErrorMessage);
            Assert.AreEqual(new DateTime(2013, 6, 3), data.TimeStamp);
            Assert.That(data is Scalar);
            Assert.AreEqual(28, (data as Scalar).Value, 1);
        }

        /// <summary>
        /// Tests requesting more than one entry and checks values correspond.
        /// The values of the ticker are approximate.
        /// </summary>
        [Test]
        public void TestRequestMultipleEntry()
        {
            global::MEFFIntegration.MEFFIntegration wrapper = new global::MEFFIntegration.MEFFIntegration();
            IMarketData[] data;
            DateTime[] dates;
            MarketDataQuery query = new MarketDataQuery();
            query.Ticker = "GRF";
            query.Date = new DateTime(2013, 6, 3);
            query.MarketDataType = typeof(Scalar).ToString();
            query.Field = "close";

            Status status = wrapper.GetTimeSeries(query, new DateTime(2013, 6, 4), out dates, out data);

            Assert.That(!status.HasErrors, status.ErrorMessage);
            Assert.AreEqual(2, data.Length);
            Assert.AreEqual(2, dates.Length);

            Assert.AreEqual(new DateTime(2013, 6, 3), data[1].TimeStamp);
            Assert.AreEqual(new DateTime(2013, 6, 4), data[0].TimeStamp);
            Assert.That(data[0] is Scalar);
            Assert.That(data[1] is Scalar);

            Assert.AreEqual(28, (data[1] as Scalar).Value, 1);
            Assert.AreEqual(29, (data[0] as Scalar).Value, 1);
        }

        /// <summary>
        /// Tests the ticker list request. This test might change
        /// if symbols starting with G are added or removed.
        /// </summary>
        [Test]
        public void TestTickerList()
        {
            global::MEFFIntegration.MEFFIntegration wrapper = new global::MEFFIntegration.MEFFIntegration();
            List<ISymbolDefinition> data = new List<ISymbolDefinition>(wrapper.SupportedTickers("G"));

            // Should contain GAS, GAM, GRF. So 3 elements.
            Assert.AreEqual(data.Count, 3);

            // Check the actual elements.
            Assert.IsTrue(data.Exists(x => (x.Name == "GAS" && x.Description == "MEFF Market Equity")));
            Assert.IsTrue(data.Exists(x => (x.Name == "GAM" && x.Description == "MEFF Market Equity")));
            Assert.IsTrue(data.Exists(x => (x.Name == "GRF" && x.Description == "MEFF Market Equity")));
        }

        /// <summary>
        /// Gets the Call price market data and checks if the request was succesful.
        /// </summary>
        [Test]
        public void TestGetCallPriceMarketData()
        {
            global::MEFFIntegration.MEFFIntegration wrapper = new global::MEFFIntegration.MEFFIntegration();

            MarketDataQuery mdq = new MarketDataQuery();
            mdq.Ticker = "BBVA";
            mdq.Date = new DateTime(2013, 07, 01);
            mdq.Market = "EU";
            mdq.Field = "close";
            mdq.MarketDataType = typeof(Fairmat.MarketData.CallPriceMarketData).ToString();
            IMarketData marketData;
            var status = wrapper.GetMarketData(mdq, out marketData);
            Assert.IsFalse(status.HasErrors);
        }
    }
}
