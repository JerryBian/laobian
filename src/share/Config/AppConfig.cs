﻿namespace Laobian.Share.Config
{
    public class AppConfig
    {
        public string SendGridApiKey { get; set; }

        public string AssetGitHubRepoApiToken { get; set; }

        public string AssetGitHubRepoBranch { get; set; }

        public string AssetRepoLocalDir { get; set; }

        public string AssetGitHubRepoOwner { get; set; }

        public string AssetGitHubRepoName { get; set; }

        public string AssetGitHubHookSecret { get; set; }

        public bool CloneAssetsDuringStartup { get; set; }

        public string AssetGitCommitUser { get; set; }

        public string AssetGitCommitEmail { get; set; }

        public string BlogAddress { get; set; }

        public bool PostUpdateScheduled { get; set; }

        public int PostUpdateAtHour { get; set; }

        public int PostUpdateEverySeconds { get; set; }

        public int ErrorLogsSendInterval { get; set; }

        public int WarningLogsSendInterval { get; set; }
    }
}
