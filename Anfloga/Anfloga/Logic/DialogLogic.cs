using Anfloga.Entities;
using Anfloga.GumRuntimes;
using FlatRedBall.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anfloga.Logic
{
    public class DialogLogic
    {
        enum DialogShownState
        {
            /// <summary>
            /// The default state the dialog box has not been shown yet.
            /// </summary>
            Hidden,
            /// <summary>
            /// The dialog box has been shown by the user moving into an automatic
            /// dialog zone, but hasn't yet been dismissed, and the user hasn't moved
            /// out of the object that showed it
            /// </summary>
            AutomaticallyShown,
            /// <summary>
            /// The dialog box was shown due to input pressed hwen the user was inside
            /// of a dialog zone.
            /// </summary>
            ExplicitlyShown,
            /// <summary>
            /// An automatic zone appeared, the user explicitly hid it - do we use this?
            /// </summary>
            ExplicitlyHidAutomaticZone
        }

        DialogShownState currentDialogShownState = DialogShownState.Hidden;

        WorldObjectEntity entityShowingDialog;

        public ActionPromptRuntime CheckActionPrompt { get; set; }

        public DialogBoxRuntime DialogBox { get; set; }


        public void Update(bool pressedDialogButton, WorldObjectEntity worldEntityCollidingWith)
        {
            switch(currentDialogShownState)
            {
                case DialogShownState.Hidden:
                    
                    if(worldEntityCollidingWith != null)
                    {
                        bool shouldShow = false;
                        if(worldEntityCollidingWith.AutomaticDialogDisplay)
                        {
                            shouldShow = true;
                            currentDialogShownState = DialogShownState.AutomaticallyShown;
                        }
                        else if(pressedDialogButton)
                        {
                            shouldShow = true;
                            currentDialogShownState = DialogShownState.ExplicitlyShown;
                        }

                        if(shouldShow)
                        {
                            DialogBox.Visible = true;
                            DialogBox.Text = LocalizationManager.Translate(worldEntityCollidingWith.DialogKey);
                            entityShowingDialog = worldEntityCollidingWith;
                        }
                    }

                    break;
                case DialogShownState.AutomaticallyShown:

                    if (worldEntityCollidingWith != entityShowingDialog)
                    {
                        // user moved out of the collision area, so hide it:
                        CloseDialog();
                    }

                    // Do we want the user to be able to automatically close out the dialog?

                    break;
                case DialogShownState.ExplicitlyShown:
                    if(pressedDialogButton)
                    {
                        CloseDialog();
                    }
                    break;
            }
        }

        private void CloseDialog()
        {
            entityShowingDialog = null;
            DialogBox.Visible = false;
            currentDialogShownState = DialogShownState.Hidden;
        }
    }
}
