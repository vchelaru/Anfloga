using Anfloga.Entities;
using Anfloga.GumRuntimes;
using FlatRedBall.Localization;
using FlatRedBall.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anfloga.Logic
{
    public class DialogLogic
    {
        #region Enums

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

        #endregion

        #region Fields/Properties

        DialogShownState currentDialogShownState = DialogShownState.Hidden;

        WorldObjectEntity entityShowingDialog;

        public ActionPromptRuntime CheckActionPrompt { get; set; }

        public DialogBoxRuntime DialogBox { get; set; }

        #endregion
 
        public void Update(bool pressedDialogButton, WorldObjectEntity worldEntityCollidingWith)
        {
            switch(currentDialogShownState)
            {
                case DialogShownState.Hidden:
                    
                    if(worldEntityCollidingWith != null)
                    {
                        bool shouldSkipConsumedDialog = worldEntityCollidingWith.IsConsumable && worldEntityCollidingWith.HasBeenConsumed;

                        bool shouldShow = false;

                        if (!shouldSkipConsumedDialog)
                        {
                            if (worldEntityCollidingWith.AutomaticDialogDisplay)
                            {
                                shouldShow = true;
                                currentDialogShownState = DialogShownState.AutomaticallyShown;
                            }
                            else if (pressedDialogButton)
                            {
                                shouldShow = true;
                                currentDialogShownState = DialogShownState.ExplicitlyShown;
                            }

                            if (shouldShow)
                            {
                                DialogBox.Visible = true;
                                worldEntityCollidingWith.HasBeenConsumed = true;
                                DialogBox.Text = LocalizationManager.Translate(worldEntityCollidingWith.DialogKey);
                                worldEntityCollidingWith.TimeDialogShown = ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
                                entityShowingDialog = worldEntityCollidingWith;
                            }
                        }
                    }

                    break;
                case DialogShownState.AutomaticallyShown:
                    bool shouldClose = false;
                    if(entityShowingDialog.AutomaticDismissTime > 0)
                    {
                        shouldClose = ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(entityShowingDialog.TimeDialogShown) >
                            entityShowingDialog.AutomaticDismissTime;
                    }
                    else if (worldEntityCollidingWith != entityShowingDialog)
                    {
                        // user moved out of the collision area, so hide it    
                        shouldClose = true;
                    }

                    if(shouldClose)
                    {
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
