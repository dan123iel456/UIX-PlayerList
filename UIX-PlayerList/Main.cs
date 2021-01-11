using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UIExpansionKit.API;
using VRC;

namespace UIX_PlayerList
{
    public class Main : MelonMod
    {
        LayoutDescription style;
        bool alphabetical;
        bool hideSelf;

        private ICustomShowableLayoutedMenu playerMenu;

		public override void OnApplicationStart()
		{
            MelonPrefs.RegisterCategory("UIXPlayerList", "UIX Player List");
            MelonPrefs.RegisterString("UIXPlayerList", "MenuStyle", "List", "Menu Style");
            MelonPrefs.RegisterBool("UIXPlayerList", "Sort", true, "Alphabetical Sort");
            MelonPrefs.RegisterBool("UIXPlayerList", "HideSelf", false, "Hide Self");
        }


		public override void VRChat_OnUiManagerInit()
		{
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Players", delegate ()
            {
                loadPlayerMenu();
                playerMenu.Show();
            });

            var menuOptions = new[]
            {
                ("List", "List"),
                ("3Grid", "3x3 Grid"),
                ("4Grid", "4x4 Grid"),
            };
            ExpansionKitApi.RegisterSettingAsStringEnum("UIXPlayerList", "MenuStyle", menuOptions);

            updateValues();

        }

		public override void OnModSettingsApplied()
		{
            updateValues();
        }

        public void updateValues()
		{
            switch (MelonPrefs.GetString("UIXPlayerList", "MenuStyle"))
            {
                case "List":
                    style = LayoutDescription.WideSlimList;
                    break;
                case "3Grid":
                    style = LayoutDescription.QuickMenu3Columns;
                    break;
                case "4Grid":
                    style = LayoutDescription.QuickMenu4Columns;
                    break;
                default:
                    style = LayoutDescription.WideSlimList;
                    break;
            }
            alphabetical = MelonPrefs.GetBool("UIXPlayerList", "Sort");
            hideSelf = MelonPrefs.GetBool("UIXPlayerList", "HideSelf");
        }


        public void loadPlayerMenu()
		{
            playerMenu = ExpansionKitApi.CreateCustomQuickMenuPage(style);

            playerMenu.AddSimpleButton("<b>~Back~</b>", delegate ()
            {
                playerMenu.Hide();
            });

            if (PlayerManager.field_Private_Static_PlayerManager_0 != null)
			{
                Player[] players = PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.ToArray();
                Player[] list = sortPlayerList(players);
                if(list.Length != 0)
				{
                    foreach (Player player in list)
					{
                        playerMenu.AddSimpleButton(player.field_Private_APIUser_0.displayName, delegate ()
                        {
                            QuickMenu.prop_QuickMenu_0.Method_Public_Void_Player_0(player);
                            playerMenu.Hide();
                        });

                    }
				}
				else
				{
                    playerMenu.AddSimpleButton("List Empty", null);
                }
			}
			else
			{
                playerMenu.AddLabel("PlayerList Null");
			}

        }

        public Player[] sortPlayerList(Player[] players)
        {
			if (alphabetical)
			{
                for (int i = 1; i < players.Length; i++)
                {
                    Player element = players[i];
                    int j;
                    for (j = i - 1; j >= 0 && element.field_Private_APIUser_0.displayName.CompareTo(players[j].field_Private_APIUser_0.displayName) <= 0; j--)
                        players[j + 1] = players[j];

                    players[j + 1] = element;
                }
            }
			if (hideSelf)
			{
                List<Player> temp = new List<Player>(); 
                foreach (Player tempPlayer in players)
				{
                    if(tempPlayer.field_Private_APIUser_0.id != VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_Player_0.field_Private_APIUser_0.id)
					{
                        temp.Add(tempPlayer);
					}
				}
                players = temp.ToArray<Player>();
			}
            
            return players;
        }

    }
}
