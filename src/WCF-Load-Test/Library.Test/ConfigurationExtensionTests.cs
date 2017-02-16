//---------------------------------------------------------------------
// <copyright file="ConfigurationExtensionTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ConfigurationExtensionTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfigurationExtensionTests
    {
        [TestMethod]
        public void ConfigurationXNullActionsReturnsNoSelectedActions()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            Assert.AreEqual<int>(0, actions.SelectedCount);
        }

        [TestMethod]
        public void ConfigurationXNullActionsReturnsNoUnselectedActions()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            Assert.AreEqual<int>(0, actions.UnselectedCount);
        }

        [TestMethod]
        public void ConfigurationXEmptyActionsReturnsNoSelectedActions()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            actions.soapAction = new SoapActionType[0];
            Assert.AreEqual<int>(0, actions.SelectedCount);
        }

        [TestMethod]
        public void ConfigurationXEmptyActionsReturnsNoUnselectedActions()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            actions.soapAction = new SoapActionType[0];
            Assert.AreEqual<int>(0, actions.UnselectedCount);
        }

        [TestMethod]
        public void ConfigurationXOneSelectedActionReturnsOneSelectedAction()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            actions.soapAction = new SoapActionType[1];
            actions.soapAction[0] = new SoapActionType();
            actions.soapAction[0].Selected = true;
            Assert.AreEqual<int>(1, actions.SelectedCount);
        }

        [TestMethod]
        public void ConfigurationXOneSelectedActionReturnsNoUnselectedActions()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            actions.soapAction = new SoapActionType[1];
            actions.soapAction[0] = new SoapActionType();
            actions.soapAction[0].Selected = true;
            Assert.AreEqual<int>(0, actions.UnselectedCount);
        }

        [TestMethod]
        public void ConfigurationXOneUnselectedActionReturnsNoSelectedActions()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            actions.soapAction = new SoapActionType[1];
            actions.soapAction[0] = new SoapActionType();
            actions.soapAction[0].Selected = false;
            Assert.AreEqual<int>(0, actions.SelectedCount);
        }

        [TestMethod]
        public void ConfigurationXOneUnselectedActionReturnsOneUnselectedAction()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            actions.soapAction = new SoapActionType[1];
            actions.soapAction[0] = new SoapActionType();
            actions.soapAction[0].Selected = false;
            Assert.AreEqual<int>(1, actions.UnselectedCount);
        }

        [TestMethod]
        public void ConfigurationXOneUnselectedActionAndOneSelectedActionReturnsOneSelectedAction()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            actions.soapAction = new SoapActionType[2];
            actions.soapAction[0] = new SoapActionType();
            actions.soapAction[1] = new SoapActionType();
            actions.soapAction[0].Selected = false;
            actions.soapAction[1].Selected = true;
            Assert.AreEqual<int>(1, actions.SelectedCount);
        }

        [TestMethod]
        public void ConfigurationXOneUnselectedActionAndOneSelectedActionReturnsOneUnselectedAction()
        {
            WcfUnitConfigurationSoapActions actions = new WcfUnitConfigurationSoapActions();
            actions.soapAction = new SoapActionType[2];
            actions.soapAction[0] = new SoapActionType();
            actions.soapAction[1] = new SoapActionType();
            actions.soapAction[0].Selected = false;
            actions.soapAction[1].Selected = true;
            Assert.AreEqual<int>(1, actions.UnselectedCount);
        }

        [TestMethod]
        public void ConfigurationXAddAssemblyToEmptyConfigurationNewAssemblyAppearsInAssemblyList()
        {
            WcfUnitConfiguration cfg = new WcfUnitConfiguration();

            cfg.AddAssembly("one");

            Assert.AreEqual<int>(1, cfg.assembly.Length);
            Assert.AreEqual<string>("one", cfg.assembly[0].fileName);
        }

        [TestMethod]
        public void ConfigurationXAddAssemblyToExistingListAppendsNewAssembly()
        {
            WcfUnitConfiguration cfg = new WcfUnitConfiguration();
            cfg.AddAssembly("one");

            cfg.AddAssembly("two");

            Assert.AreEqual<int>(2, cfg.assembly.Length);
            Assert.AreEqual<string>("one", cfg.assembly[0].fileName);
            Assert.AreEqual<string>("two", cfg.assembly[1].fileName);
        }

        [TestMethod]
        public void ConfigurationXRemoveSecondAssemblyFromListOfTwoAssembliesRemovesAssembly()
        {
            WcfUnitConfiguration cfg = new WcfUnitConfiguration();
            cfg.AddAssembly("one");
            cfg.AddAssembly("two");

            cfg.RemoveAssembly(1);

            Assert.AreEqual<int>(1, cfg.assembly.Length);
            Assert.AreEqual<string>("one", cfg.assembly[0].fileName);
        }

        [TestMethod]
        public void ConfigurationXRemoveFirstAssemblyFromListOfTwoAssembliesRemovesAssembly()
        {
            WcfUnitConfiguration cfg = new WcfUnitConfiguration();
            cfg.AddAssembly("one");
            cfg.AddAssembly("two");

            cfg.RemoveAssembly(0);

            Assert.AreEqual<int>(1, cfg.assembly.Length);
            Assert.AreEqual<string>("two", cfg.assembly[0].fileName);
        }

        [TestMethod]
        public void ConfigurationXRemoveLastAssemblyFromListLeavesZeroLengthList()
        {
            WcfUnitConfiguration cfg = new WcfUnitConfiguration();
            cfg.AddAssembly("one");

            cfg.RemoveAssembly(0);

            Assert.AreEqual<int>(0, cfg.assembly.Length);
        }

        [TestMethod]
        public void ConfigurationXRemoveAssemblyFromNullListThrowsInvalidOperationException()
        {
            WcfUnitConfiguration cfg = new WcfUnitConfiguration();

            try
            {
                cfg.RemoveAssembly(0);
                Assert.Fail("Expected exception not thrown");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void ConfigurationXRemoveAssemblyFromEmptyListThrowsInvalidOperationException()
        {
            WcfUnitConfiguration cfg = new WcfUnitConfiguration();
            cfg.AddAssembly("one");
            cfg.RemoveAssembly(0);

            try
            {
                cfg.RemoveAssembly(0);
                Assert.Fail("Expected exception not thrown");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void ConfigurationXCloneMakesDeepCopy()
        {
            WcfUnitConfiguration cfg1 = new WcfUnitConfiguration();
            cfg1.clientTrace = true;
            cfg1.serviceTrace = false;
            cfg1.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            cfg1.testMethodMode = TestMethodMode.IncludeIndividualOperations;
            cfg1.AddAssembly("abc.def");
            cfg1.soapActions = new WcfUnitConfigurationSoapActions();
            cfg1.soapActions.soapActionMode = SoapActionMode.Include;
            cfg1.soapActions.soapAction = new SoapActionType[1] { new SoapActionType() };
            cfg1.soapActions.soapAction[0].action = "action1";
            cfg1.soapActions.soapAction[0].Selected = true;
            cfg1.parser = null;

            WcfUnitConfiguration cfg2 = cfg1.Clone();

            Assert.AreNotSame(cfg1, cfg2);
            Assert.IsTrue(cfg2.clientTrace);
            Assert.IsFalse(cfg2.serviceTrace);
            Assert.AreEqual<OperationTimerMode>(OperationTimerMode.IncludeOperationTimers, cfg2.operationTimerMode);
            Assert.AreEqual<TestMethodMode>(TestMethodMode.IncludeIndividualOperations, cfg2.testMethodMode);
            Assert.AreNotSame(cfg1.assembly, cfg2.assembly);
            Assert.AreEqual<int>(1, cfg2.assembly.Length);
            Assert.AreNotSame(cfg1.assembly[0], cfg2.assembly[0]);
            Assert.AreEqual<string>("abc.def", cfg2.assembly[0].fileName);
            Assert.AreNotSame(cfg1.soapActions, cfg2.soapActions);
            Assert.AreEqual<SoapActionMode>(SoapActionMode.Include, cfg2.soapActions.soapActionMode);
            Assert.AreNotSame(cfg1.soapActions.soapAction, cfg2.soapActions.soapAction);
            Assert.AreEqual<int>(1, cfg2.soapActions.soapAction.Length);
            Assert.AreNotSame(cfg1.soapActions.soapAction[0], cfg2.soapActions.soapAction[0]);
            Assert.AreEqual<string>("action1", cfg2.soapActions.soapAction[0].action);
            Assert.IsTrue(cfg2.soapActions.soapAction[0].Selected);
            Assert.IsNull(cfg2.parser);

            // Now set different values for value types only and make sure these are cloned correctly
            cfg1.clientTrace = false;
            cfg1.serviceTrace = true;
            cfg1.operationTimerMode = OperationTimerMode.NoOperationTimers;
            cfg1.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            cfg1.soapActions.soapActionMode = SoapActionMode.Exclude;
            cfg1.soapActions.soapAction[0].Selected = false;

            cfg2 = cfg1.Clone();

            Assert.IsFalse(cfg2.clientTrace);
            Assert.IsTrue(cfg2.serviceTrace);
            Assert.AreEqual<OperationTimerMode>(OperationTimerMode.NoOperationTimers, cfg2.operationTimerMode);
            Assert.AreEqual<TestMethodMode>(TestMethodMode.ScenarioMethodOnly, cfg2.testMethodMode);
            Assert.AreEqual<SoapActionMode>(SoapActionMode.Exclude, cfg2.soapActions.soapActionMode);
            Assert.IsFalse(cfg2.soapActions.soapAction[0].Selected);
        }

        [TestMethod]
        public void ConfigurationXCloneWithNullArraysMakesDeepCopy()
        {
            WcfUnitConfiguration cfg1 = new WcfUnitConfiguration();
            cfg1.clientTrace = true;
            cfg1.serviceTrace = false;
            cfg1.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            cfg1.testMethodMode = TestMethodMode.IncludeIndividualOperations;
            cfg1.soapActions = new WcfUnitConfigurationSoapActions();
            cfg1.soapActions.soapActionMode = SoapActionMode.Include;

            WcfUnitConfiguration cfg2 = cfg1.Clone();

            Assert.IsNull(cfg2.assembly);
            Assert.IsNull(cfg2.soapActions.soapAction);
        }

        [TestMethod]
        public void ConfigurationXCloneCopiesParserSetting()
        {
            WcfUnitConfiguration cfg1 = new WcfUnitConfiguration();
            cfg1.parser = new typeType();
            cfg1.parser.assembly = "abc";
            cfg1.parser.type = "def";

            WcfUnitConfiguration cfg2 = cfg1.Clone();

            Assert.IsNotNull(cfg2.parser);
            Assert.AreNotSame(cfg1.parser, cfg2.parser);
            Assert.AreEqual<string>("abc", cfg2.parser.assembly);
            Assert.AreEqual<string>("def", cfg2.parser.type);
        }
    }
}
