﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using UMT.Sitecore.Configuration;
using UMT.Sitecore.Diagnostics;
using UMT.Sitecore.Models;

namespace UMT.Sitecore.Pipelines.ExtractItems
{
    public class MapMediaItems
    {
        public virtual void Process(ExtractItemsArgs args)
        {
            Assert.ArgumentNotNull(args, nameof(args));
            Assert.ArgumentNotNull(args.SourceMediaItems, nameof(args.SourceMediaItems));

            UMTLog.Info($"{nameof(MapMediaItems)} pipeline processor started");

            var folderPath = CreateFileExtractFolder(DateTime.Now);
            args.TargetMediaLibrary = GetTargetMediaLibrary(args.SourceMediaLibrary);
            args.TargetMediaItems = GetTargetMediaItems(args.SourceMediaItems, args.SourceMediaLibrary, folderPath);
            UMTLog.Info($"{nameof(MapMediaItems)}: " + args.TargetMediaItems.Count + " items have been mapped");

            UMTLog.Info($"{nameof(MapMediaItems)} pipeline processor finished");
        }

        protected virtual MediaLibrary GetTargetMediaLibrary(MediaMap sourceMediaLibrary)
        {
            var mediaLibrary = new MediaLibrary
            {
                LibraryGUID = sourceMediaLibrary.Id,
                LibraryName = sourceMediaLibrary.Name,
                LibraryDisplayName = sourceMediaLibrary.DisplayName,
                LibraryDescription = sourceMediaLibrary.Description,
                LibraryFolder = sourceMediaLibrary.LibraryFolder
            };

            return mediaLibrary;
        }

        protected virtual List<MediaFile> GetTargetMediaItems(IList<MediaItem> items, MediaMap sourceMediaLibrary, string folderPath)
        {
            var mappedItems = new List<MediaFile>();

            foreach (var item in items)
            {
                mappedItems.Add(MapToTargetItem(item, sourceMediaLibrary, folderPath));
            }

            return mappedItems;
        }

        protected virtual MediaFile MapToTargetItem(MediaItem mediaItem, MediaMap sourceMediaLibrary, string folderPath)
        {
            int.TryParse(mediaItem.InnerItem["Width"], out var width);
            int.TryParse(mediaItem.InnerItem["Height"], out var height);
            var fileExtension = $".{mediaItem.Extension}";
            var fileName = mediaItem.Name.EndsWith(fileExtension)
                ? mediaItem.Name
                : $"{mediaItem.Name}.{mediaItem.Extension}";
            var targetItem = new MediaFile
            {
                FileGUID = mediaItem.ID.Guid,
                FileDescription = mediaItem.Description,
                FileExtension = fileExtension,
                FileName = mediaItem.Name,
                FilePath = mediaItem.MediaPath.Trim('/'),
                FileTitle = mediaItem.Title,
                FileMimeType = mediaItem.MimeType,
                FileImageHeight = height,
                FileImageWidth = width,
                FileCreatedWhen = mediaItem.InnerItem.Statistics.Created,
                FileModifiedWhen = mediaItem.InnerItem.Statistics.Updated,
                FileLibraryGuid = sourceMediaLibrary.Id
            };

            var fileFolder = $"{folderPath}{mediaItem.MediaPath.Substring(0, mediaItem.MediaPath.LastIndexOf(mediaItem.Name))}"; 
            var dataSourcePath = SaveFile(mediaItem, fileFolder, fileName);
            if (!string.IsNullOrEmpty(dataSourcePath))
            {
                targetItem.DataSourcePath = dataSourcePath.Replace(folderPath, ".\\Files");
            }
            else
            {
                UMTLog.Warn($"Media file {targetItem.FilePath} ({mediaItem.ID}) was not saved to the output folder.");
            }
            
            return targetItem;
        }

        protected virtual string SaveFile(MediaItem mediaItem, string folderPath, string fileName)
        {
            using (var stream = mediaItem.GetMediaStream())
            {
                if (stream != null)
                {
                    if (folderPath.Length + fileName.Length > 256)
                    {
                        folderPath = folderPath.Substring(0, 256 - fileName.Length).TrimEnd(' ', '/');
                    }
                    
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    
                    var filePath = MainUtil.MakeFilePath(folderPath, fileName);

                    using (var file = File.Create(filePath))
                    {
                        stream.CopyTo(file);
                    }

                    return filePath;
                }
            }

            return string.Empty;
        }
        
        protected virtual string CreateFileExtractFolder(DateTime extractDateTime)
        {
            var extractFolderName = extractDateTime.ToString(UMTSettings.DataFolderDateFormat);
            var folderPath = MainUtil.MapPath(UMTSettings.DataFolder + $"/{extractFolderName}/Files");
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }
    }
}