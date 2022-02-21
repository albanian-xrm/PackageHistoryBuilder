using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;

internal class GitClient
{
    const string GIT_IGNORE = ".gitignore";
    Signature committer = new Signature("AlbanianXrm", "albanian@xrm.al", DateTimeOffset.Now);
    private string outputPath;

    public GitClient(string outputPath)
    {
        this.outputPath = outputPath;
    }

    internal void Initialize()
    {
        if (!Repository.IsValid(outputPath))
        {
            Repository.Init(outputPath);
            new FileInfo(GIT_IGNORE).CopyTo(Path.Combine(outputPath, GIT_IGNORE));
            CommitChanges("Added .gitignore");
        }
        Console.WriteLine($"Initialized {outputPath}");
    }

    internal IEnumerable<Tag> GetRepositoryTags()
    {
        using (var repo = new Repository(outputPath))
        {
            foreach (var tag in repo.Tags)
            {
                yield return tag;
            }
        }
    }

    internal void CommitChanges(string message, string tag = null)
    {
        using (var repo = new Repository(outputPath))
        {
            Commands.Stage(repo, "*");
            if (!repo.RetrieveStatus().IsDirty)
            {
                Console.WriteLine("Nothing to commit " + message);
                return;
            }
            repo.Commit(message, committer, committer);
            if (tag != null)
            {
                repo.ApplyTag(tag);
            }
            Console.WriteLine($"Commited {message}");
        }
    }
}


