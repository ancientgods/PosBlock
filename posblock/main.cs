using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;

namespace posblock
{
    [ApiVersion(1, 16)]
    public class PosBlock : TerrariaPlugin
    {
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "Ancientgods"; }
        }
        public override string Name
        {
            get { return "block measurer"; }
        }

        public override string Description
        {
            get { return "Gives you tools to measure distance between blocks"; }
        }
        public PosBlock(Main game)
            : base(game)
        {
            Order = 1;
        }
       
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tshock.world.modify",block, "block"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        private void block(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! valid syntax: /block set 1|2/calc/center>");
                args.Player.SendWarningMessage("[Warning] /block center will spawn a tile (can destroy stuff)");
                return;
            }
            switch (args.Parameters[0].ToLower())
            {
                case "set":
                    int choice = 0;
                    if (args.Parameters.Count == 2 &&
                            int.TryParse(args.Parameters[1], out choice) &&
                            choice >= 1 && choice <= 2)
                    {
                        args.Player.SendMessage("Hit a block to Set Point " + choice, Color.Yellow);
                        args.Player.AwaitingTempPoint = choice;
                    }
                    else
                    {
                        args.Player.SendMessage("Invalid syntax! Proper syntax: /block set <1/2>", Color.Red);
                    }
                    break;
                case "calc":
                    int x = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X);
                    int y = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y);
                    args.Player.SendInfoMessage(string.Format("Difference between X (horizontal): {0}", x));
                    args.Player.SendInfoMessage(string.Format("Difference between Y (vertical): {0}", y));
                    break;
                case "center":
                    if(!args.Player.Group.HasPermission("block.center"))
                    {
                        args.Player.SendErrorMessage("You do not have permission to use this command!");
                        return;
                    }
                    if (args.Player.TempPoints[0].X < 1 && args.Player.TempPoints[0].Y < 1 || args.Player.TempPoints[1].X < 1 && args.Player.TempPoints[1].Y < 1)
                    {
                        args.Player.SendErrorMessage("You do not have any points set up!");
                        return;
                    }
                    int X = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X) / 2;
                    int Y = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y) / 2;
                    int rx = Math.Max(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X) - X;
                    int ry = Math.Max(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y) - Y;
                    args.Player.TempPoints[0] = Point.Zero;
                    args.Player.TempPoints[1] = Point.Zero;
                    Main.tile[rx, ry].active(true);
                    Main.tile[rx, ry].type = (byte)160;
                    args.Player.SendTileSquare(rx,ry);
                    args.Player.Teleport(rx * 16, (ry - 3) * 16);
                    args.Player.SendSuccessMessage("Teleported you to the middle of your selected points!");

                    break;
                default:
                    args.Player.SendErrorMessage("Invalid syntax! valid syntax: /block set 1|2/calc/center>");
                    args.Player.SendWarningMessage("[Warning] /block center will spawn a tile (can destroy stuff)");
                    return;
            }
        }
    }
}