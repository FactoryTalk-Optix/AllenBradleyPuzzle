#region Using directives
using FTOptix.Core;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UAManagedCore;
using FTOptix.OPCUAServer;
#endregion

public class PuzzleLogic : BaseNetLogic {
    private Random rnd = new Random();
    private PeriodicTask piecesChecker;
    private static int piecesTolerance = 20;

    public override void Start() {
        // Generate puzzle pieces randomly
        InitializePuzzle();
    }

    public override void Stop() {
        // Insert code to be executed when the user-defined logic is stopped
        piecesChecker?.Dispose();
    }

    [ExportMethod]
    public void InitializePuzzle() {
        // Prepare game field
        Owner.Get<Label>("CongratsMessage").Visible = false;
        Owner.Get<Rectangle>("PuzzleArea").Visible = true;
        // Reset game tick (if any)
        piecesChecker?.Dispose();
        // Prepare game area
        Rectangle dstPage = Owner.Get<Rectangle>("PuzzleArea");
        for (int i = 0; i < 9; i++) {
            bool newPiece = false;
            PuzzlePiece singlePiece = dstPage.Get<PuzzlePiece>("Piece" + i) ?? null;
            if (singlePiece == null) {
                singlePiece = InformationModel.Make<PuzzlePiece>("Piece" + i);
                newPiece = true;
            }
            string localPath = Path.DirectorySeparatorChar + "imgs" + Path.DirectorySeparatorChar + "Puzzle" + Path.DirectorySeparatorChar + "Piece" + (i + 1).ToString() + ".png";
            singlePiece.Get<Image>("PuzzleImage").Path = "ns=" + singlePiece.NodeId.NamespaceIndex.ToString() + ";%PROJECTDIR%" + localPath;
                                                         
            Log.Info(singlePiece.Get<Image>("PuzzleImage").Path.Uri);
            singlePiece.Rotation = rnd.Next(0, 4) * 90;
            singlePiece.Width = 160;
            singlePiece.Height = 160;
            singlePiece.LeftMargin = rnd.Next(540, 1000);
            singlePiece.TopMargin = rnd.Next(0, 320);
            if (newPiece) {
                dstPage.Add(singlePiece);
            }
        }
        // Start game timer
        piecesChecker = new PeriodicTask(CheckPuzzlePos, 500, LogicObject);
        piecesChecker.Start();
    }

    public void CheckPuzzlePos() {
        Rectangle dstPage = Owner.Get<Rectangle>("PuzzleArea");
        int goodPieces = 0;
        // Loop per each piece to align them to the grid
        for (int i = 0; i < 9; i++) {
            var singlePiece = dstPage.Get<PuzzlePiece>("Piece" + i);
            int expTop = (i / 3) * 160;
            int expLeft = (i % 3) * 160;
            string logMsg = "Left: " + singlePiece.LeftMargin + "/" + expLeft + " - Top: " + singlePiece.TopMargin + "/" + expTop;
            // Check pieces positioning
            if (singlePiece.LeftMargin >= expLeft - piecesTolerance && singlePiece.LeftMargin <= expLeft + piecesTolerance &&
                singlePiece.TopMargin >= expTop - piecesTolerance && singlePiece.TopMargin <= expTop + piecesTolerance) {
                logMsg += " - OK";
                // Snap pieces to grid
                singlePiece.LeftMargin = expLeft;
                singlePiece.TopMargin = expTop;
                // Check if rotation is correct
                if (singlePiece.Rotation== 0) {
                    ++goodPieces;
                }
            }
            Log.Debug("PuzzleLogic.CheckPuzzlePos.Piece" + i, logMsg);
            // Check if all the pieces are in the right spot
            if (goodPieces >= 9) {
                Owner.Get<Label>("CongratsMessage").Visible = true;
                Owner.Get<Rectangle>("PuzzleArea").Visible = false;
                piecesChecker?.Dispose();
            }
        }
    }
}
