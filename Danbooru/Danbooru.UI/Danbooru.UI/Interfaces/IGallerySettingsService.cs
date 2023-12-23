using Danbooru.UI.Models;

namespace Danbooru.UI.Interfaces;

public interface IGallerySettingsService
{
    /// <summary>
    /// Retrieves the current gallery options.
    /// </summary>
    /// <returns>The current gallery options.</returns>
    GalleryOptionsModel GetGalleryOptions();

    T GetSetting<T>(Func<GalleryOptionsModel, T> getAction);

    /// <summary>
    /// Updates the gallery settings based on a provided action.
    /// </summary>
    /// <param name="updateAction">The action that updates the settings.</param>
    void UpdateSetting(Action<GalleryOptionsModel> updateAction);

    /// <summary>
    /// Generates a query string based on the current gallery settings.
    /// </summary>
    /// <returns>A query string representing the current settings.</returns>
    string GenerateQueryString();

    /// <summary>
    /// Navigates to a new URL using the current gallery settings.
    /// </summary>
    void NavigateWithUpdatedSettings();
}
