#region Using directives
using FTOptix.NetLogic;
using System;
using UAManagedCore;
#endregion

public class PuzzlePieceLogic : BaseNetLogic {
    private int clickMillis = 0;
    public override void Start() {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop() {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void MouseDownEvent() {
        clickMillis = DateTime.Now.Millisecond;
    }
    [ExportMethod]
    public void MouseUpEvent() {
        if (DateTime.Now.Millisecond - clickMillis <= 500) {
            RotatePiece();
        }
    }

    public void RotatePiece() {
        var curAngle = Owner.GetVariable("Rotation");
        int newAngle = curAngle.Value;
        newAngle = (newAngle + 90) % 360;
        curAngle.Value = newAngle;
    }
}
