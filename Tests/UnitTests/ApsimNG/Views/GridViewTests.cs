﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;
using Models;
using NUnit.Framework;
using UserInterface.Presenters;
using UserInterface.Views;
using APSIM.Shared.Utilities;
using Models.Core;
using System.Reflection;
using UnitTests.ApsimNG.Utilities;
using UserInterface.Classes;

namespace UnitTests.ApsimNG.Views
{
    [TestFixture]
    public class GridViewTests
    {
        private ExplorerPresenter explorerPresenter;

        [OneTimeSetUp]
        public void OpenTestFileInTab()
        {
            explorerPresenter = UITestUtilities.OpenResourceFileInTab(Assembly.GetExecutingAssembly(),
                                                    "UnitTests.ApsimNG.Resources.SampleFiles.BasicSimulation.apsimx");
        }

        [OneTimeTearDown]
        public void CloseTab()
        {
            // Close the tab we opened. This assumes that this is the only open tab.
            UITestsMain.MasterPresenter.CloseTab(0, onLeft: true);
        }

        /// <summary>
        /// This test ensures that a keypress while a cell is selected will cause the cell to enter edit mode.
        /// </summary>
        [Test]
        public void EnsureKeyPressInitiatesEditing()
        {
            // Click on clock node.
            explorerPresenter.SelectNode(".Simulations.Simulation.Clock");
            GtkUtilities.WaitForGtkEvents();

            GridView grid = explorerPresenter.CurrentRightHandView as GridView;
            if (grid == null)
                throw new Exception("Clock view is not a GridView");

            // Click on top-right cell - this will be in the value column, and so will be editable.
            GtkUtilities.GetTreeViewCoordinates(grid.Grid, 0, 1, out int x, out int y);
            GtkUtilities.Click(grid.Grid, Gdk.EventType.ButtonPress, Gdk.ModifierType.None, GtkUtilities.ButtonPressType.LeftClick, x, y);

            // Grid should not be in edit mode at this point.
            Assert.IsFalse(grid.IsUserEditingCell);

            // Type the letter 'a' now that the cell is selected.
            GtkUtilities.SendKeyPress(grid.Grid, 'a');

            // Grid should now be in edit mode.
            Assert.IsTrue(grid.IsUserEditingCell);
        }

        /// <summary>
        /// This test ensures that double clicking a cell will cause the cell to enter edit mode.
        /// </summary>
        [Test]
        public void EnsureDoubleClickInitiatesEditing()
        {
            // Click on clock node.
            explorerPresenter.SelectNode(".Simulations.Simulation.Clock");
            GtkUtilities.WaitForGtkEvents();

            GridView grid = explorerPresenter.CurrentRightHandView as GridView;
            if (grid == null)
                throw new Exception("Clock view is not a GridView");

            // We want to click on a cell, but this requires coordinates.
            GtkUtilities.GetTreeViewCoordinates(grid.Grid, 0, 1, out int x, out int y);

            // Grid should not be in edit mode at this point.
            Assert.IsFalse(grid.IsUserEditingCell);

            // Double-click on the top-right cell using the coordinates.
            GtkUtilities.Click(grid.Grid, Gdk.EventType.TwoButtonPress, Gdk.ModifierType.None, GtkUtilities.ButtonPressType.LeftClick, x, y);

            // Grid should now be in edit mode.
            Assert.IsTrue(grid.IsUserEditingCell);
        }
    }
}
