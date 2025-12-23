// SPDX-License-Identifier: GPL-3.0-or-later
namespace DsmSuite.Analyzer.Cpp.IncludeResolve
{
    public interface IIncludeResolveStrategy
    {
        IList<IncludeCandidate> GetCandidates(string relativeIncludeFilename);
        IList<string> Resolve(string sourceFilename, string relativeIncludeFilename, IList<IncludeCandidate> candidates);
    }
}
