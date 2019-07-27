using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MyAddin
{
    public class MyAddinClass
    {
        // define menu constants
        const string menuHeader = "-&Face插件";
        const string menuImport = "&导入";
        const string menuExport = "&导出";
        const string menuOpen = "&打开项目";
        const string menuNewFace = "&新建Face项目";
     
        ///
        /// Called Before EA starts to check Add-In Exists
        /// Nothing is done here.
        /// This operation needs to exists for the addin to work
        ///
        /// <param name="Repository" />the EA repository
        /// a string
        public String EA_Connect(EA.Repository Repository)
        {
            //No special processing required.
            return "a string";
        }


        ///
        /// Called when user Clicks Add-Ins Menu item from within EA.
        /// Populates the Menu with our desired selections.
        /// Location can be "TreeView" "MainMenu" or "Diagram".
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        ///
        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {

            switch (MenuName)
            {
                // defines the top level menu option
                case "":
                    return menuHeader;
                // defines the submenu options
                case menuHeader:
                    string[] subMenus = { menuImport, menuExport, menuOpen, menuNewFace };
                    return subMenus;
            }

            return "";
        }

        ///
        /// returns true if a project is currently opened
        ///
        /// <param name="Repository" />the repository
        /// true if a project is opened in EA
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }


        ///
        /// Called once Menu has been opened to see what menu items should active.
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        /// <param name="ItemName" />the name of the menu item
        /// <param name="IsEnabled" />boolean indicating whethe the menu item is enabled
        /// <param name="IsChecked" />boolean indicating whether the menu is checked
        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            IsEnabled = false;
            if (IsProjectOpen(Repository) && ItemName == menuExport)
            {
                IsEnabled = true;
                return;
            }
            switch (ItemName)
            {
                case menuImport:
                    IsEnabled = true;
                    break;
                case menuOpen:
                    IsEnabled = true;
                    break;
                case menuNewFace:
                    IsEnabled = true;
                    break;
            }
        }

        ///
        /// Called when user makes a selection in the menu.
        /// This is your main exit point to the rest of your Add-in
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        /// <param name="ItemName" />the name of the selected menu item
        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            switch (ItemName)
            {
           
                // user has clicked the menuHello menu option
                case menuImport:
                    this.importXML(Repository);
                    break;
                // user has clicked the menuGoodbye menu option
                case menuExport:
                    this.exportXML(Repository);
                    break;
                case menuOpen:
                    this.openEAP(Repository);
                    break;
                case menuNewFace:
                    this.newFaceProject(Repository);
                    break;
            }
        }

        public void EA_OnOutputItemClicked(EA.Repository Repository, string TabName, string LineText, long ID)
        {
            EA.Package package =  Repository.GetTreeSelectedPackage();
            MessageBox.Show(String.Format("Tab name: %s, Line Text: %s, id: %s", TabName, LineText, ID));
        }

        public void EA_OnOutputItemDoubleClicked(EA.Repository Repository, string TabName, string LineText, long ID)
        {
            MessageBox.Show(String.Format("Tab name: %s, Line Text: %s, id: %s", TabName, LineText, ID));
        }

        ///
        /// Import XML
        ///
        private void importXML(EA.Repository Repository)
        {
            Form importForm = new ImportForm(Repository);
            importForm.Show();
        }

        private void openEAP(EA.Repository Repository)
        {
            Form openForm = new OpenEAPForm(Repository);
            openForm.Show();
        }

        private void newFaceProject(EA.Repository Repository)
        {
            Form newFaceForm = new NewFaceForm(Repository);
            newFaceForm.Show();
        }

        ///
        /// Export XML
        ///
        private void exportXML(EA.Repository Repository)
        {
            Form exportForm = new ExportForm(Repository);
            exportForm.Show();
        }

        ///
        /// EA calls this operation when it exists. Can be used to do some cleanup work.
        ///
        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

    }
}
