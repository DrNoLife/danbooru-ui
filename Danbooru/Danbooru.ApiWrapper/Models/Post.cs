using System.Text.Json.Serialization;

namespace Danbooru.ApiWrapper.Models;

public class Post
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("uploader_id")]
    public int UploaderId { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = String.Empty;

    [JsonPropertyName("md5")]
    public string Md5 { get; set; } = String.Empty;

    [JsonPropertyName("last_comment_bumped_at")]
    public object LastCommentBumpedAt { get; set; } = String.Empty;

    [JsonPropertyName("rating")]
    public string Rating { get; set; } = String.Empty;

    [JsonPropertyName("image_width")]
    public int Width { get; set; }

    [JsonPropertyName("image_height")]
    public int Height { get; set; }

    [JsonPropertyName("tag_string")]
    public string TagString { get; set; } = String.Empty;

    [JsonPropertyName("fav_count")]
    public int FavCount { get; set; }

    [JsonPropertyName("file_ext")]
    public string FileExt { get; set; } = String.Empty;

    [JsonPropertyName("last_noted_at")]
    public object LastNotedAt { get; set; } = String.Empty;

    [JsonPropertyName("parent_id")]
    public int? ParentId { get; set; }

    [JsonPropertyName("has_children")]
    public bool HasChildren { get; set; }

    [JsonPropertyName("approver_id")]
    public int? ApproverId { get; set; }

    [JsonPropertyName("tag_count_general")]
    public int TagCountGeneral { get; set; }

    [JsonPropertyName("tag_count_artist")]
    public int TagCountArtist { get; set; }

    [JsonPropertyName("tag_count_character")]
    public int TagCountCharacter { get; set; }

    [JsonPropertyName("tag_count_copyright")]
    public int TagCountCopyright { get; set; }

    [JsonPropertyName("file_size")]
    public int FileSize { get; set; }

    [JsonPropertyName("up_score")]
    public int UpScore { get; set; }

    [JsonPropertyName("down_score")]
    public int DownScore { get; set; }

    [JsonPropertyName("is_pending")]
    public bool IsPending { get; set; }

    [JsonPropertyName("is_flagged")]
    public bool IsFlagged { get; set; }

    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("tag_count")]
    public int TagCount { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("is_banned")]
    public bool IsBanned { get; set; }

    [JsonPropertyName("pixiv_id")]
    public int? PixivId { get; set; }

    [JsonPropertyName("last_commented_at")]
    public object? LastCommentedAt { get; set; }

    [JsonPropertyName("has_active_children")]
    public bool HasActiveChildren { get; set; }

    [JsonPropertyName("bit_flags")]
    public int BitFlags { get; set; }

    [JsonPropertyName("tag_count_meta")]
    public int TagCountMeta { get; set; }

    [JsonPropertyName("has_large")]
    public bool HasLarge { get; set; }

    [JsonPropertyName("has_visible_children")]
    public bool HasVisibleChildren { get; set; }

    [JsonPropertyName("media_asset")]
    public MediaAsset? MediaAsset { get; set; }

    [JsonPropertyName("tag_string_general")]
    public string TagStringGeneral { get; set; } = String.Empty;

    [JsonPropertyName("tag_string_character")]
    public string TagStringCharacter { get; set; } = String.Empty;

    [JsonPropertyName("tag_string_copyright")]
    public string TagStringCopyright { get; set; } = String.Empty;

    [JsonPropertyName("tag_string_artist")]
    public string TagStringArtist { get; set; } = String.Empty;

    [JsonPropertyName("tag_string_meta")]
    public string TagStringMeta { get; set; } = String.Empty;

    [JsonPropertyName("file_url")]
    public string FileUrl { get; set; } = String.Empty;

    [JsonPropertyName("large_file_url")]
    public string LargeFileUrl { get; set; } = String.Empty;

    [JsonPropertyName("preview_file_url")]
    public string PreviewFileUrl { get; set; } = String.Empty;

    private List<string>? _tagsGeneral;
    public List<string> TagsGeneral 
    {
        get
        {
            if(_tagsGeneral is null)
            {
                _tagsGeneral = GetTagsListFromString(TagStringGeneral);
            }

            return _tagsGeneral.Count == 1 && String.IsNullOrEmpty(_tagsGeneral[0]) 
                ? new List<string>() 
                : _tagsGeneral;
        }
    }

    private List<string>? _tagsCharacter;
    public List<string> TagsCharacter
    {
        get
        {
            if (_tagsCharacter is null)
            {
                _tagsCharacter = GetTagsListFromString(TagStringCharacter);
            }

            return _tagsCharacter.Count == 1 && String.IsNullOrEmpty(_tagsCharacter[0])
                ? new List<string>()
                : _tagsCharacter;
        }
    }

    private List<string>? _tagsArtist;
    public List<string> TagsArtist
    {
        get
        {
            if (_tagsArtist is null)
            {
                _tagsArtist = GetTagsListFromString(TagStringArtist);
            }

            return _tagsArtist.Count == 1 && String.IsNullOrEmpty(_tagsArtist[0])
                ? new List<string>()
                : _tagsArtist;
        }
    }

    private List<string>? _tagsCopyright;
    public List<string> TagsCopyright
    {
        get
        {
            if (_tagsCopyright is null)
            {
                _tagsCopyright = GetTagsListFromString(TagStringCopyright);
            }

            return _tagsCopyright.Count == 1 && String.IsNullOrEmpty(_tagsCopyright[0])
                ? new List<string>()
                : _tagsCopyright;
        }
    }

    private List<string>? _tagsMeta;
    public List<string> TagsMeta
    {
        get
        {
            if (_tagsMeta is null)
            {
                _tagsMeta = GetTagsListFromString(TagStringMeta);
            }

            return _tagsMeta.Count == 1 && String.IsNullOrEmpty(_tagsMeta[0])
                ? new List<string>()
                : _tagsMeta;
        }
    }

    private List<string> GetTagsListFromString(string tagString)
    {
        var tags = tagString.Split(' ');
        for (int i = 0; i < tags.Length; i++)
        {
            var tag = tags[i];
            tag.Replace('_', ' ');
        }

        return tags.ToList();
    }
}
