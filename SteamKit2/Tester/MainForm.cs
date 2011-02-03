﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SteamKit2;
using System.Diagnostics;

namespace Tester
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        public void UpdateCallbacks()
        {
            CallbackMsg callback = SteamContext.SteamClient.GetCallback();

            if ( callback == null )
                return;

            SteamContext.SteamClient.FreeLastCallback();

            if ( callback is ConnectedCallback )
            {
                SteamContext.SteamUser.LogOn( SteamContext.LoginDetails );
            }

            if ( callback is FriendsListCallback )
            {
                uint friendCount = SteamContext.SteamFriends.GetFriendCount();

                for ( uint x = 0 ; x < friendCount ; x++ )
                {
                    SteamID friendId = SteamContext.SteamFriends.GetFriendByIndex( x );

                    Friend frnd = new Friend();
                    frnd.FriendID = friendId;

                    lbUsers.Items.Add( frnd );
                }
            }

            if ( callback is FriendMsgCallback )
            {
                FriendMsgCallback friendMsg = (FriendMsgCallback)callback;

                MessageDialog md = FindOrCreateMsgDlg( friendMsg.Sender );
                md.RecvMessage( friendMsg );
            }

            if ( callback is PersonaStateCallback )
            {
                // refresh the box so we can grab some names
                lbUsers.RefreshItems();
            }

            if ( callback is LoginKeyCallback )
            {
                SteamContext.SteamFriends.SetPersonaState( EPersonaState.Online );
            }
        }

        MessageDialog FindOrCreateMsgDlg( SteamID friendId )
        {
            foreach ( MessageDialog md in this.OwnedForms )
            {
                if ( md.FriendID == friendId )
                    return md;
            }

            MessageDialog msgDlg = new MessageDialog( friendId );
            msgDlg.Show( this );

            return msgDlg;
        }

        private void lbUsers_MouseDoubleClick( object sender, MouseEventArgs e )
        {
            if ( lbUsers.SelectedItems.Count == 0 )
                return;

            Friend frnd = ( Friend )lbUsers.SelectedItem;

            FindOrCreateMsgDlg( frnd.FriendID );
        }
    }

    struct Friend
    {
        public ulong FriendID;

        public override string ToString()
        {
            return SteamContext.SteamFriends.GetFriendPersonaName( FriendID );
        }
    }
}