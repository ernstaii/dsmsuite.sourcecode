﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Analyzer.VisualStudio.Test.Util;
using DsmSuite.Analyzer.VisualStudio.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DsmSuite.Analyzer.VisualStudio.Test.VisualStudio
{
    [TestClass]
    public class VcxProjectFileTest
    {
        [TestMethod]
        public void TestSolutionFolder()
        {
            VcxProjectFile projectFile = CreateProjectFile();
            Assert.AreEqual("Analyzers.VisualStudioAnalyzer", projectFile.SolutionFolder);
        }

        [TestMethod]
        public void TestProjectName()
        {
            VcxProjectFile projectFile = CreateProjectFile();
            Assert.AreEqual("DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj", projectFile.ProjectName);
        }
        
        [TestMethod]
        public void TestProjectIncludeDirectoriesFound()
        {
            VcxProjectFile projectFile = CreateProjectFile();
            projectFile.Analyze();
            Assert.AreEqual(7, projectFile.ProjectIncludeDirectories.Count);

            ImmutableHashSet<string> includes = projectFile.ProjectIncludeDirectories.ToImmutableHashSet();
            Assert.IsTrue(includes.Contains(TestData.TestDataDirectory));
            string includeInProjectFile = Path.Combine(TestData.TestDataDirectory, "DirA");
            Assert.IsTrue(includes.Contains(includeInProjectFile));
            string includeInProjectFileUsingProperty = Path.Combine(TestData.TestDataDirectory, "DirB");
            Assert.IsTrue(includes.Contains(includeInProjectFileUsingProperty));
            string includeInPropertyFile = Path.Combine(TestData.TestDataDirectory, "DirC");
            Assert.IsTrue(includes.Contains(includeInPropertyFile));
            string includeInPropertyFileUsingProperty = Path.Combine(TestData.TestDataDirectory, "DirD");
            Assert.IsTrue(includes.Contains(includeInPropertyFileUsingProperty));
            string includeInPropertyFileUsingProgramming = Path.Combine(TestData.TestDataDirectory, "DirE");
            Assert.IsTrue(includes.Contains(includeInPropertyFileUsingProgramming));
            string includeExternal = Path.Combine(TestData.TestDataDirectory, "DirExternal");
            Assert.IsTrue(includes.Contains(includeExternal));
        }

        [TestMethod]
        public void TestExternalIncludeDirectoriesFound()
        {
            string externalDir1 = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\DsmSuite.Analyzer.VisualStudio.Test\External1"));
            string externalDir2 = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\DsmSuite.Analyzer.VisualStudio.Test\External2"));
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            analyzerSettings.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory { Path = externalDir1, ResolveAs = "External1"});
            analyzerSettings.ExternalIncludeDirectories.Add(new ExternalIncludeDirectory { Path = externalDir2, ResolveAs = "External2"});
            VcxProjectFile projectFile = CreateProjectFile(analyzerSettings);
            projectFile.Analyze();
            Assert.AreEqual(2, projectFile.ExternalIncludeDirectories.Count);

            ImmutableHashSet<string> includes = projectFile.ExternalIncludeDirectories.ToImmutableHashSet();
            Assert.IsTrue(includes.Contains(externalDir1));
            Assert.IsTrue(includes.Contains(externalDir2));
        }

        [TestMethod]
        public void TestSystemIncludeDirectoriesFound()
        {
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            VcxProjectFile projectFile = CreateProjectFile(analyzerSettings);
            projectFile.Analyze();
            Assert.AreEqual(5, projectFile.SystemIncludeDirectories.Count);

            ImmutableHashSet<string> includes = projectFile.SystemIncludeDirectories.ToImmutableHashSet();
            Assert.IsTrue(includes.Contains(@"C:\Program Files (x86)\Windows Kits\8.1\Include\um"));
            Assert.IsTrue(includes.Contains(@"C:\Program Files (x86)\Windows Kits\8.1\Include\shared"));
            Assert.IsTrue(includes.Contains(@"C:\Program Files (x86)\Windows Kits\10\Include\10.0.10240.0\ucrt"));
            Assert.IsTrue(includes.Contains(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\include"));
            Assert.IsTrue(includes.Contains(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\atlmfc\include"));
        }

        [TestMethod]
        public void TestSourceFilesFound()
        {
            VcxProjectFile projectFile = CreateProjectFile();
            projectFile.Analyze();

            HashSet<string> sourceFilenames = GetSourceFiles(projectFile);
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.h")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA1.cpp")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA2.h")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirA\ClassA2.cpp")));

            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirB\ClassB1.h")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirB\ClassB1.cpp")));

            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirC\ClassC1.h")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirC\ClassC1.cpp")));

            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirD\ClassD1.h")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirD\ClassD1.cpp")));

            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirE\ClassE1.h")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirE\ClassE1.cpp")));

            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirClones\Identical\ClassA1.cpp")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirClones\Identical\CopyClassA1.cpp")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirClones\NotIdentical\ClassA1.cpp")));

            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirIDL\IInterface1.idl")));
            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirIDL\IInterface2.idl")));

            Assert.IsTrue(sourceFilenames.Contains(Path.Combine(TestData.TestDataDirectory, "targetver.h")));

            Assert.AreEqual(24, projectFile.SourceFiles.Count); // 18 files plus 6 IDL generated files
        }

        [TestMethod]
        public void TestGeneratedFileRelationsFound()
        {
            VcxProjectFile projectFile = CreateProjectFile();
            projectFile.Analyze();

            HashSet<string> consumerNames = new HashSet<string>();
            HashSet<string> providerNames = new HashSet<string>();
            foreach (GeneratedFileRelation generatedFileRelation in projectFile.GeneratedFileRelations)
            {
                providerNames.Add((generatedFileRelation.Provider.Name));
                consumerNames.Add(generatedFileRelation.Consumer.Name);
            }

            Assert.IsTrue(providerNames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirIDL\IInterface1.idl"))); 
            Assert.IsTrue(providerNames.Contains(Path.Combine(TestData.TestDataDirectory, @"DirIDL\IInterface2.idl")));
            Assert.AreEqual(2, providerNames.Count);

            Assert.IsTrue(consumerNames.Contains(Path.Combine(TestData.TestDataDirectory, @"IdlOutput\IInterface1.h"))); // Using %(Filename).h setting
            Assert.IsTrue(consumerNames.Contains(Path.Combine(TestData.TestDataDirectory, @"IdlOutput\IInterface1_i.c"))); // Using empty setting
            Assert.IsTrue(consumerNames.Contains(Path.Combine(TestData.TestDataDirectory, @"IdlOutput\MyTypeLibrary.tlb"))); // Using explicit setting

            Assert.IsTrue(consumerNames.Contains(Path.Combine(TestData.TestDataDirectory, @"IdlOutput\IInterface2.h"))); // Using %(Filename).h setting
            Assert.IsTrue(consumerNames.Contains(Path.Combine(TestData.TestDataDirectory, @"IdlOutput\IInterface2_i.c"))); // Using empty setting
            Assert.IsTrue(consumerNames.Contains(Path.Combine(TestData.TestDataDirectory, @"IdlOutput\DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.tlb"))); // Using explicit setting

            Assert.AreEqual(6, consumerNames.Count);

            Assert.AreEqual(6, projectFile.GeneratedFileRelations.Count);
        }

        private VcxProjectFile CreateProjectFile()
        {
            AnalyzerSettings analyzerSettings = AnalyzerSettings.CreateDefault();
            return CreateProjectFile(analyzerSettings);
        }

        private VcxProjectFile CreateProjectFile(AnalyzerSettings analyzerSettings)
        {
            string solutionFolder = "Analyzers.VisualStudioAnalyzer";
            string testDataDirectory = TestData.TestDataDirectory;
            string solutionDir = Path.GetFullPath(Path.Combine(testDataDirectory, @"..\"));
            string projecPath = Path.GetFullPath(Path.Combine(testDataDirectory, "DsmSuite.Analyzer.VisualStudio.Test.Data.Cpp.vcxproj"));
            return new VcxProjectFile(solutionFolder, solutionDir, projecPath, analyzerSettings);
        }

        private static HashSet<string> GetSourceFiles(VcxProjectFile projectFile)
        {
            HashSet<string> sourceFiles = new HashSet<string>();
            foreach (SourceFile sourceFile in projectFile.SourceFiles)
            {
                sourceFiles.Add(sourceFile.SourceFileInfo.FullName);
            }
            return sourceFiles;
        }
    }
}