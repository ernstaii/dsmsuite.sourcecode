using DsmSuite.Analyzer.C4.Settings;
using DsmSuite.Analyzer.C4.Test.Utils;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Analyzer.Model.Interface;
using System.Reflection;

namespace DsmSuite.Analyzer.C4.Test.Analysis
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void TestAnalyze()
        {
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            analyzerSettings.Input.Workspace = Path.Combine(TestData.RootDirectory, TestData.TestWorkspace1);
            analyzerSettings.Transformation.IgnoredNames = new List<string>();

            IDsiModel model = new DsiModel("Test", analyzerSettings.Transformation.IgnoredNames, Assembly.GetExecutingAssembly());
            C4.Analysis.Analyzer analyzer = new C4.Analysis.Analyzer(model, analyzerSettings, null);

            analyzer.Analyze();

            // Main elements
            var elementUser = model.FindElementByName("User");
            Assert.IsNotNull(elementUser);

            var elementAdmin = model.FindElementByName("Admin");
            Assert.IsNotNull(elementAdmin);

            var elementSoftwareSystem = model.FindElementByName("Software System");
            Assert.IsNotNull(elementSoftwareSystem);

            var elementSoftwareSystemApp1 = model.FindElementByName("Software System.Web Application 1");
            Assert.IsNotNull(elementSoftwareSystemApp1);

            var elementSoftwareSystemApp2 = model.FindElementByName("Software System.Web Application 2");
            Assert.IsNotNull(elementSoftwareSystemApp2);

            var elementSoftwareSystemApp3 = model.FindElementByName("Software System.Web Application 3");
            Assert.IsNotNull(elementSoftwareSystemApp3);

            var elementUsersController = model.FindElementByName("Software System.Web Application 2.Users Controller");
            Assert.IsNotNull(elementUsersController);

            var elementPermissionsController = model.FindElementByName("Software System.Web Application 2.Permissions Controller");
            Assert.IsNotNull(elementPermissionsController);

            var elementDatabase1 = model.FindElementByName("Software System.Database1");
            Assert.IsNotNull(elementDatabase1);

            var elementDatabase2 = model.FindElementByName("Software System.Database2");
            Assert.IsNotNull(elementDatabase2);

            var elementWebserver1 = model.FindElementByName("Deployments.Development.Web Server 1");
            Assert.IsNotNull(elementWebserver1);

            var elementContainerInstance1 = model.FindElementByName("Deployments.Development.Web Server 1.webapp1instance");
            Assert.IsNotNull(elementContainerInstance1);

            var elementContainerInstance2 = model.FindElementByName("Deployments.Development.Web Server 1.webapp2instance");
            Assert.IsNotNull(elementContainerInstance2);

            var elementWebserver2 = model.FindElementByName("Deployments.Development.Web Server 2");
            Assert.IsNotNull(elementWebserver2);

            var elementContainerInstance3 = model.FindElementByName("Deployments.Development.Web Server 2.webapp3instance");
            Assert.IsNotNull(elementContainerInstance3);

            var elementDatabaseServer = model.FindElementByName("Deployments.Development.Database Server");
            Assert.IsNotNull(elementDatabaseServer);

            var elementContainerInstance4 = model.FindElementByName("Deployments.Development.Web Server 2.Redis Server");
            Assert.IsNotNull(elementContainerInstance4);

            var elementContainerInstance5 = model.FindElementByName("Deployments.Development.Web Server 2.webapp3instance");
            Assert.IsNotNull(elementContainerInstance5);

            // Main relations
            Assert.IsTrue(model.DoesRelationExist(elementUser.Id, elementSoftwareSystem.Id));
            Assert.IsTrue(model.DoesRelationExist(elementUser.Id, elementSoftwareSystemApp1.Id));

        }
    }
}