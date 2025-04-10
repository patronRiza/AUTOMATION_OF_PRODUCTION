﻿using AutomationOfPostprocessing;
using AutomationOfPostprocessing.Services.FileSystem;
using NXOpen;
using NXOpen.BlockStyler;
using System;
using System.IO;

public class PostprocessDialog
{
    private static Session theSession = null;
    private static UI theUI = null;
    private static PostprocessorService postprocessorSupplier = null;
    private static BlockDialog theDialog;
    private static NXLogger logger;
    private NXOpen.BlockStyler.Group group1;
    private ListBox list_box0;
    private NXOpen.BlockStyler.Group group;
    private FolderSelection nativeFolderBrowser0;
    private StringBlock string0;
    private string postName;
    private string outputDir;
    private string extention;

    public PostprocessDialog(Session session, UI ui, NXLogger logger)
    {
        try
        {
            theSession = session;
            theUI = ui;
            PostprocessDialog.logger = logger;
            postprocessorSupplier = new PostprocessorService();
            string ugiiUserDir = Environment.GetEnvironmentVariable("UGII_CUSTOM_DIALOGS_DIR");
            string theDlxFileName = Path.Combine(ugiiUserDir, "application", "patroN.dlx");
            theDialog = theUI.CreateDialog(theDlxFileName);
            theDialog.AddApplyHandler(new BlockDialog.Apply(apply_cb));
            theDialog.AddOkHandler(new BlockDialog.Ok(ok_cb));
            theDialog.AddUpdateHandler(new BlockDialog.Update(update_cb));
            theDialog.AddInitializeHandler(new BlockDialog.Initialize(initialize_cb));
            theDialog.AddDialogShownHandler(new BlockDialog.DialogShown(dialogShown_cb));
        }
        catch (Exception ex)
        {
            PostprocessDialog.logger.LogError(ex);
            throw ex;
        }
    }

    public static void UnloadLibrary(string arg)
    {
        theDialog?.Dispose();
        try
        {
            theSession = null;
            theUI = null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex);
        }
    }

    public int Show()
    {
        try
        {
            theDialog.Show();
        }
        catch (Exception ex)
        {
            logger.LogError(ex);
            return 1;
        }
        return 0;
    }

    public void Dispose()
    {
        if (theDialog != null)
        {
            theDialog.Dispose();
            theDialog = null;
        }
    }

    public void initialize_cb()
    {
        try
        {
            group1 = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group1");
            list_box0 = (ListBox)theDialog.TopBlock.FindBlock("list_box0");
            list_box0.SingleSelect = true;

            group = (NXOpen.BlockStyler.Group)theDialog.TopBlock.FindBlock("group");
            nativeFolderBrowser0 = (FolderSelection)theDialog.TopBlock.FindBlock("nativeFolderBrowser0");
            string0 = (StringBlock)theDialog.TopBlock.FindBlock("string0");

            nativeFolderBrowser0.Path = @"C:\";

            theDialog.AddUpdateHandler(new BlockDialog.Update(update_cb));
        }
        catch (Exception ex)
        {
            logger.LogError(ex);
        }
    }

    public void dialogShown_cb()
    {
        try
        {
            list_box0.SetListItems(new string[0]);
            var posts = postprocessorSupplier.GetAvailablePostprocessors().ToArray();

            if (posts.Length == 0)
            {
                logger.LogInfo("Нет установленных постпроцессоров");
                return;
            }

            Array.Sort(posts);

            list_box0.SetListItems(posts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex);
        }
    }

    public int apply_cb()
    {
        int errorCode = 0;
        try
        {
            int[] selectedIndices = list_box0.GetSelectedItems();

            if (selectedIndices.Length == 0)
            {
                logger.LogWarning("Не выбран постпроцессор!");
                return 1;
            }

            string[] posts = list_box0.GetListItems();
            postName = posts[selectedIndices[0]];
            outputDir = nativeFolderBrowser0.Path;
            extention = string0.Value;
        }
        catch (Exception ex)
        {
            errorCode = 1;
            logger.LogError(ex);
        }
        return errorCode;
    }

    public int update_cb(UIBlock block)
    {
        try
        {
            if (block == list_box0)
            {
                int[] selectedIndices = list_box0.GetSelectedItems();
                if (selectedIndices.Length == 1)
                {
                    string[] allPosts = list_box0.GetListItems();
                    string selectedPost = allPosts[selectedIndices[0]];
                    string extention = string.Empty;

                    try
                    {
                        extention = postprocessorSupplier.GetExtention(selectedPost);
                    }
                    catch (Exception)
                    {
                        extention = "NC";
                    }

                    string0.Value = extention;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex);
        }
        return 0;
    }

    public int ok_cb()
    {
        int errorCode = 0;
        try
        {
            errorCode = apply_cb();
        }
        catch (Exception ex)
        {
            errorCode = 1;
            logger.LogError(ex);
        }
        return errorCode;
    }

    public PropertyList GetBlockProperties(string blockID)
    {
        PropertyList plist = null;
        try
        {
            plist = theDialog.GetBlockProperties(blockID);
        }
        catch (Exception ex)
        {
            logger.LogError(ex);
        }
        return plist;
    }

    public string GetSelectedPostprocessor() => postName;

    public string GetOutputDirectory() => outputDir;

    public string GetPostprocessorExtention() => extention;
}
