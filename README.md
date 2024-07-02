[//]: # "[![Contributors][contributors-shield]][contributors-url]"
[//]: # "[![Forks][forks-shield]][forks-url]"
[//]: # "[![Stargazers][stars-shield]][stars-url]"
[//]: # "[![Issues][issues-shield]][issues-url]"
[//]: # "[![MIT License][license-shield]][license-url]"
[//]: # "[![Discord][discussion-shield]][discussion-url]"

<!-- ABOUT THE PROJECT -->
# Xperience by Kentico: Sitecore Migration Tool

The Xperience by Kentico: Sitecore Migration Tool automates data export from Sitecore to be imported into Xperience by Kentico.

## Prerequisites & Compatibility

### Source

The migration currently supports Sitecore version 9.0 (Initial Release) or newer.

### Target

* This tool is periodically updated to support migration to the **latest version** of Xperience by Kentico. However, there may be delays between Xperience by Kentico releases and toolkit updates.
  * Currently, Xperience by Kentico **28.3.0** is tested and supported.
* The target instance's database and file system must be accessible from the environment where you run the Sitecore Migration Tool.
* The target instance's database must be empty except for data from the source instance created by previous runs of the Sitecore Migration Tool to avoid conflicts and inconsistencies.
  * Only some global objects can be created in the target instance upfront and then mapped during the configuration of Sitecore module:
    * Language(s)
    * Website channel(s)
    * Media library(s)

## Supported data and limitations

The Sitecore Migration Tool does not transfer all data available in the source instance. Xperience by Kentico currently provides a smaller, more focused set of features. As a result, some objects are discarded or migrated to a suitable alternative.

Currently, this tool supports the following types of data:

* **Sites**
  * The Sitecore Migration Tool doesn't identify websites in Sitecore automatically, the [website channel(s)](https://docs.xperience.io/x/34HFC) need to be defined in the config of Sitecore module.
* **Cultures**
  * The set of cultures used across all Sitecore sites in the source gets mapped to a [language](https://docs.xperience.io/x/OxT_Cw) in the _Languages_ application. This is defined in the config of Sitecore module.
* **Data Templates**
  * This tool attempts to map the Sitecore field's _Type_ to an appropriate equivalent in Xperience by Kentico. This mapping is not always possible and can be amended in the config of Sitecore module.
  * This tool does not allow you to manually select the _Data Templates_ that will be migrated. Instead, when exporting the selected content from Sitecore CMS tree this tool will automatically detect all _Data Templates_ used within this tree and export them.    
  * This tool does not support migration of _Source_ restrictions defined in the Sitecore.
  * If migrated _Data Templates_ have _Presentaition Details_ defined for _Standard Values_, then _Content Type_ in Xperience by Kentico will have the **Page** feature enabled.
  * Sitecore _Data Templates_ inheritance is not maintained during the migration. For each _Data Template_ this tool exports all the fields from all inherited templates.   
* **Content Items**
  * This tool allows migration of _Content Items_ from Sitecore into Xperience by Kentico as both _Pages_ and _Content Hub_ items.
    * By default all items are migrated as Pages.
    * In the Sitecore module config you can define which specific _Data Templates_ will be migrated to _Content Hub_ instead. 
  * This tool only migrates the current latest version of each page, regardless whether it's published or not.
    * It is possible to specify Sitecore database name, therefore selecting Web database will export only the latest published versions of pages.
  * Each page gets assigned under its corresponding website channel selected during the export.
  * Page permissions (ACLs) are not migrated.
  * Sitecore _Presentation Details Renderings_ are **NOT** migrated as _Page Builder_ widgets into Xperience by Kentico.
    * The current version of module is primarily focussed on structured content migration. Page builder components migration is not yet supported, although this can be done as a customization to this module. 
* **Media libraries and media files**
  * During the run of the export task it is possible to specify which _Media Library_ in Xperience by Kentico the media files will be migrated to.
  * Media library permissions are not migrated.

### Unsupported data

The following types of data exist in Sitecore but are currently **not supported** by the Sitecore Migration Tool:

* **Branch Templates**
  * Not supported.
* **Forms**
  * Not supported.
* **Users**
  * Not supported.
* **Roles**
  * Not supported.
* **Contacts**
  * Not supported.
* **Activities**
  * Not supported.


## Getting Started

### Source

* The source of the migration data must be a Sitecore XM or XP instance, version 9.0 (Initial Release) or newer.
* The tool runs in the context of a Sitecore application, therefore it should be deployed to a Sitecore CM or Standalone instance that has access to a database with the content you want to export. For the best performance it is recommended to take a backup of Sitecore `master` database and connect your local Sitecore instance to it. However, it is also possible to install the UMT package in a cloud environment and run the export process there.

### Installation

Follow the steps below to install the Xperience by Kentico: Sitecore Migration Tool:

1. Download the package from the repository [Releases](https://github.com/Kentico/sitecore-migration-toolkit/releases).
2. Install the package to your Sitecore instance by going to the **Control Panel** → **Install a package** and following the installation wizard steps. The package will add `.config` and `.dll` files so it will require a restart of the Sitecore application. 
3. Configure the options in the `App_Config/Include/UMT/UMT.Sitecore.config` configuration file. See [Configuration](#configuration) section below for reference.

### Configuration

<table>
    <thead>
        <tr>
            <th>Configuration section</th>
            <th>Configuration element</th>
            <th>Description</th>
            <th>Notes</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td rowspan="10"><code>&lt;settings&gt;</code></td>
            <td>UMT.Database</td>
            <td>Database name that will be used for extracting the data from. It should be a Sitecore database name linked to a valid connection string.</td>
            <td>The default value is <code>master</code></td>
        </tr>
        <tr>
            <td>UMT.DataFolder</td>
            <td>Folder path on the file system that will be used for storing the generated output.</td>
            <td>The default value is <code>$(dataFolder)/UMT</code></td>
        </tr>
        <tr>
            <td>UMT.DataFolderDateFormat</td>
            <td>Date-based name format for the folder that will be created for each run.</td>
            <td>The default value is <code>yyyy-MM-dd HH-mm-ss</code></td>
        </tr>
        <tr>
            <td>UMT.ExportMediaAsUrls</td>
            <td>This setting allows switching between file-based and URL media extracts. When <code>true</code>, media binary files will not be created on the file system and instead they will be created as URLs, otherwise each file will be saved to the output folder. Consider setting this to <code>true</code> if you, for example, run exports in a cloud environment and have issues downloading a large number of media files.</td>
            <td>The default value is <code>false</code></td>
        </tr>
        <tr>
            <td>UMT.ExportMediaAsUrls.ServerUrl</td>
            <td>This setting allows overriding the hostname for generated media URLs when using the option <code>UMT.ExportMediaAsUrls</code>. Leave it empty to use the current Sitecore instance settings.</td>
            <td>The default value is <code>https://legacysite/</code></td>
        </tr>
        <tr>
            <td>UMT.ExportMediaAsUrls.SiteName</td>
            <td>This setting specifies the Sitecore site name for generating media URLs when using the option <code>UMT.ExportMediaAsUrls</code>.</td>
            <td>The default value is <code>LegacySite</code></td>
        </tr>
        <tr>
            <td>UMT.MaxFilePathLength</td>
            <td>Maximum allowed file path on the file system, the export will truncate paths and file names longer than that when <code>UMT.TrimLongMediaFolderPaths</code> is set to <code>true</code>.</td>
            <td>The default value is <code>256</code></td>
        </tr>
        <tr>
            <td>UMT.MediaLocationForExport</td>
            <td>Folder path for the exported media files when <code>UMT.ExportMediaAsUrls</code> is set to <code>false</code>.</td>
            <td>The default value is <code>{outputFolder}\Files</code></td>
        </tr>
        <tr>
            <td>UMT.MediaLocationForJson</td>
            <td>Relative or absolute folder path that will be used for files in the generated JSON when <code>UMT.ExportMediaAsUrls</code> is set to <code>false</code>. Leave it empty to use the automatically generated absolute path of exported files.</td>
            <td>The default value is <code>..\Import\Files</code></td>
        </tr>
        <tr>
            <td>UMT.RichTextMediaLinkFormat</td>
            <td>Format for transforming media URLs that are refenreced in Rich Text fields.</td>
            <td>The default value is <code>~/getmedia/{0}/{1}.{2}</code> where <code>{0}</code> is media item ID, <code>{1}</code> is file name, and <code>{2}</code> is file extension.</td>
        </tr>
        <tr>
            <td>UMT.TrimLongMediaFolderPaths</td>
            <td>Enables truncation of long media paths on the file system.</td>
            <td>The default value is <code>true</code></td>
        </tr>
        <tr>
            <td><code>&lt;pipelines&gt;</code></td>
            <td><code>&lt;umt.ExtractContent&gt;</code></td>
            <td>The pipeline responsible for reading content from Sitecore, mapping it to UMT models and serializing as JSON files to the file system. <br/>If you would like to customize any of the processors in this pipeline or introduce a new processor to extract additional data or do custom transformations, you can patch processors in this pipeline.</td>
            <td>A custom processor can be added as a new element under <code>&lt;umt.ExtractContent&gt;</code>. Processors run in the same order as they are listed in the config file. </td>
        </tr>
        <tr>
            <td rowspan="9"><code>&lt;umt&gt;</code></td>
            <td><code>&lt;channelMapping&gt;</code></td>
            <td>List of channels that will be available for selection when running an export. Each channel has a list of attributes corresponding to <a href="https://docs.xperience.io/xp/developers-and-admins/configuration/website-channel-management" target="_blank">channel fields in Xperience by Kentico</a>. If a channel does not exist, it will be created automatically when importing data into Xperience by Kentico. Channel ID and website ID are defined in the config so that subsequent exports and imports refer to the same channels within Xperience by Kentico. The attribute <code>sitecoreSiteName</code> is required for correct generation of relative page URLs.</td>
            <td>You must have at least one channel as it will be used for linking content types and content items to it.</td>
        </tr>
        <tr>
            <td><code>&lt;languageMapping&gt;</code></td>
            <td>List of languages for mapping between Sitecore languages (sourceId) and Xperience by kentico languages (targetId). If you have any existing languages in the target Xperience by Kentico instance, add them to this list.</td>
            <td></td>
        </tr>
        <tr>
            <td rowspan="2"><code>&lt;contentMapping&gt;</code></td>
            <td><code>&lt;pageRoots&gt;</code> is the list of page subtrees in your Sitecore instance. This list is used for reference field mapping and automatic resolving of Content Item vs Page Item reference fields. If a linked item in a reference field is not under one of these roots, the reference field will use Content Item reference format.</td>
            <td></td>
        </tr>
        <tr>            
            <td><code>&lt;excludedSubtrees&gt;</code> is a list of content paths that will be skipped and excluded from the export.</td>
            <td>For example, you may want to exclude some non-content config items such as sitemaps or Sitecore-specific settings.</td>
        </tr>
        <tr>
            <td><code>&lt;mediaMapping&gt;</code></td>
            <td>List of media libraries available for selection when running a media export. </td>
            <td>You must have at least one media library because the exported media items will be linked to it.</code></td>
        </tr>
        <tr>
            <td rowspan="2"><code>&lt;templateMapping&gt;</code></td>
            <td><code>&lt;excludedTemplates&gt;</code> is a list of Sitecore templates that will be excluded from the export. Items based on these templates will be excluded as well. If an excluded item has any child items, they will be remapped to the closest available parent item.</td>
            <td></td>
        </tr>
        <tr>
            <td><code>&lt;contentHubTemplates&gt;</code> defines a list of Sitecore templates that will be created as Content Hub content items in Xperience by Kentico.</td>
            <td>Consider using <code>&lt;contentHubTemplates&gt;</code> for templates that are designed for reusable and multichannel content, especially if this content normally sits outside of <code>&lt;pageRoots&gt;</code>.</td>
        </tr>
        <tr>
            <td><code>&lt;fieldTypeMapping&gt;</code></td>
            <td>List of standard Sitecore fields and their corresponding column types and control names in Xperience by Kentico. The <code>type</code> attribute references a converter class that will be used for converting field values of each field type. If a field type is not included in this list, then it will be skipped during export.</td>
            <td>If you have any custom field types in Sitecore and would like to include them in the export, add a <code>&lt;fieldType&gt;</code> element to the list. You can write a custom converter class or use one of the existing converters.</td>
        </tr>
        <tr>
            <td><code>&lt;fieldMapping&gt;</code></td>
            <td><code>&lt;excludedFields&gt;</code> is a list of Sitecore fields that will be excluded from the export.</td>
            <td>Most of the fields from the standard Sitecore template are excluded to create smaller export files and cleaner structure in your target Xperience by Kentico instance. Consider excluding any fields that are not relevant for the target instance.</td>
        </tr>
        <tr>
            <td><code>&lt;log4net&gt;</code></td>
            <td><code>&lt;appender&gt;</code> and <code>&lt;logger&gt;</code></td>
            <td>Standard <code>&lt;log4net&gt;</code> configuration for UMT log files.</td>
            <td>By default UMT logs will be written to <code>$(dataFolder)/logs/UMT.log.{date}.txt</code></td>
        </tr>
    </tbody>
</table>


## Usage

### Exporting Data

1. Navigate to the URL `/sitecore/admin/UMT.aspx`.
2. Select a channel from the list, enter root paths and languages you would like to export. 
3. Optionally, select a media library and enter media folder paths you would like to export. Content and media exports can be created in one run or separately.
4. Click the button **Run export** and wait for the process to finish. The current status and progress of the export job will be updated automatically and shown on the same page.
5. Once the process is finished, you can copy the generated JSON files and use them for data import into your Xperience by Kentico instance. Generated JSON files are stored in the folder `App_Data/UMT/` by default.

## Support

This contribution has **Full support by 7-day bug-fix policy**.

See [`SUPPORT.md`](https://github.com/Kentico/.github/blob/main/SUPPORT.md#full-support) for more information.

For any security issues see [`SECURITY.md`](https://github.com/Kentico/.github/blob/main/SECURITY.md).
