﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using concord.Output;
using concord.Output.Dto;
using concord.RazorTemplates.Models;
using concord.Tests.Framework;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using RazorEngine.Templating;

namespace concord.Tests.Parsing
{
    [TestFixture]
    public class ResultsTemplateWriterTests : InteractionContext<ResultsTemplateWriter>
    {
        //private ResultsTemplateWriter _classUnderTest;
        private TemplateService _templateService;

        [SetUp]
        public void SetUp()
        {
            _templateService = new TemplateService();
            //_classUnderTest = MockRepository.GeneratePartialMock<ResultsTemplateWriter>(_templateService);

            UseInstanceFor<ITemplateService>(_templateService);
            //MockFor<IHtmlGanttChart>()
            //    .Stub(x => x.SeperateIntoLines(null))
            //    .IgnoreArguments()
            //    .WhenCalled(x =>
            //    {
            //        var list = (IEnumerable<RunStats>) x.Arguments[0];
            //        if (list == null)
            //        {
            //            x.ReturnValue = new List<List<RunStats>>();
            //            return;
            //        }
            //        x.ReturnValue = list
            //            .GroupBy(g => g.GetHashCode() % 4)
            //            .Select(g => g.ToList())
            //            .ToList();
            //    })
            //    .Return(null);
            UseInstanceFor<IHtmlGanttChart>(new HtmlGanttChart());
        }

        [Test]
        public void Should_build_template()
        {
            //Arrange
            
            //Act
            var fancyResults = new FancyResults { TotalRuntime = TimeSpan.FromSeconds(78274) };
            var output = ClassUnderTest.OutputResults(fancyResults);

            //Assert
            output.Should().NotBeNullOrWhiteSpace();
            output.Should().Contain(fancyResults.TotalRuntime.ToString());

            Debug.WriteLine(output);
        }

        [Test, Ignore("Specific to my enviroment")]
        public void Should_build_template_using_actual_data()
        {
            //Arrange
            CopyTemplates();
            var testData = JsonConvert.DeserializeObject<RunStatsCollection>(DownloadOrderDataArtifact())
                                      .Records.ToArray();

            //Act
            var fancyResults = new FancyResults
            {
                TotalRuntime = testData.Max(x => x.EndTime),
                Runners = testData.Where(x => x.IsCurrentRun),
                SkippedTests = testData.Where(x => !x.IsCurrentRun).Select(x => x.Name).ToList()
            };
            var output = ClassUnderTest.OutputResults(fancyResults, @"C:\temp\concordTestResults.html");

            //Assert
            output.Should().NotBeNullOrWhiteSpace();
            output.Should().Contain(fancyResults.TotalRuntime.ToString());

            Debug.WriteLine(output);
        }

        public void CopyTemplates()
        {
            var templateDestination = Path.Combine(Environment.CurrentDirectory, "RazorTemplates");
            //Comes from \concord.texts\bin\Debug...
            var templateSource = Path.GetFullPath("..\\..\\..\\concord\\RazorTemplates");

            if (Directory.Exists(templateDestination) && Directory.Exists(templateSource))
            {
                foreach (var template in Directory.EnumerateFiles(templateSource, "*.cshtml"))
                {
                    File.Copy(template, Path.Combine(templateDestination, Path.GetFileName(template)), true);
                }
            }
            else
            {
                throw new Exception("Unable to find RazorTemplates, are these the right paths?"
                                    + "\nDestination: " + templateDestination
                                    + "\nSource: " + templateSource);
            }
        }

        public string DownloadOrderDataArtifact()
        {
            var TEAMCITY_SERVER = "snail:88";
            var APK_JOB = "bt142";
            var APK_PATH = "OrderData.json.html";

            var TeamCity7 = "app/rest/builds/{1}/artifacts/files/{2}";
            var TeamCity8 = "app/rest/builds/{1}/artifacts/content/{2}";


            var buildLocator = string.Format("buildType:(id:{0}),canceled:false", APK_JOB);
            var path = string.Format("http://{0}/guestAuth/" + TeamCity7, TEAMCITY_SERVER, buildLocator, APK_PATH);

            using (var wc = new WebClient())
            {
                try
                {
                    return wc.DownloadString(path);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed downloading: " + path, ex);
                }
            }
        }
    }
}