namespace Laobian.Share.Setting
{
    public class BlogSetting : CommonSetting
    {
        #region redonly repo

        [SettingConfig("GITHUB_READONLY_REPO_API_TOKEN", IsRequired = true)]
        public string GitHubReadonlyRepoApiToken { get; set; }

        [SettingConfig("GITHUB_READONLY_REPO_NAME", IsRequired = true)]
        public string GitHubReadonlyRepoName { get; set; }

        [SettingConfig("GITHUB_READONLY_REPO_OWNER", IsRequired = true)]
        public string GitHubReadonlyRepoOwner { get; set; }

        [SettingConfig("GITHUB_READONLY_REPO_BRANCH", IsRequired = true)]
        public string GitHubReadonlyRepoBranch { get; set; }

        [SettingConfig("GITHUB_READONLY_REPO_LOCAL_DIR", IsRequired = true)]
        public string GitHubReadonlyRepoLocalDir { get; set; }

        #endregion

        #region read & write repo

        [SettingConfig("GITHUB_READ_WRITE_REPO_API_TOKEN", IsRequired = true)]
        public string GitHubReadWriteRepoApiToken { get; set; }

        [SettingConfig("GITHUB_READ_WRITE_REPO_NAME", IsRequired = true)]
        public string GitHubReadWriteRepoName { get; set; }

        [SettingConfig("GITHUB_READ_WRITE_REPO_OWNER", IsRequired = true)]
        public string GitHubReadWriteRepoOwner { get; set; }

        [SettingConfig("GITHUB_READ_WRITE_REPO_BRANCH", IsRequired = true)]
        public string GitHubReadWriteRepoBranch { get; set; }

        [SettingConfig("GITHUB_READ_WRITE_REPO_LOCAL_DIR", IsRequired = true)]
        public string GitHubReadWriteRepoLocalDir { get; set; }

        [SettingConfig("GITHUB_READ_WRITE_REPO_COMMIT_USER")]
        public string GitHubReadWriteRepoCommitUser { get; set; }

        [SettingConfig("GITHUB_READ_WRITE_REPO_COMMIT_EMAIL")]
        public string GitHubReadWriteRepoCommitEmail { get; set; }

        [SettingConfig("METADATA_POST_FILE_NAME", DefaultValue = "metadata/post.json")]
        public string PostMetadataFileName { get; set; }

        [SettingConfig("METADATA_TAG_FILE_NAME", DefaultValue = "metadata/tag.json")]
        public string TagFilePath { get; set; }

        [SettingConfig("POST_ACCESS_DIR_NAME", DefaultValue = "access")]
        public string AccessDir { get; set; }

        [SettingConfig("POST_COMMENT_DIR_NAME", DefaultValue = "comment")]
        public string CommentDir { get; set; }

        [SettingConfig("LOG_DIR_NAME", DefaultValue = "log")]
        public string LogDir { get; set; }

        #endregion
    }
}