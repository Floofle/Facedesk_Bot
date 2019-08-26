﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace FaceDesk_Bot.FD_MainModules
{
  class FunModule : ModuleBase<SocketCommandContext>
  {
    public static List<string> BallPositiveResponses;
    public static List<string> BallNegativeResponses;
    //includes disasterous
    public static List<string> BallNeutralResponses;

    public static List<List<string>> BallAllResponses;

    private Random ballRandom = new Random();

    [Command("8ball")]
    [Summary("Shakes the 8ball.")]
    public async Task BallShake()
    {
      string eightballPath = Path.Combine(EntryPoint.RunningFolder, "8ball");
      string rawPosResponses = File.ReadAllText(Path.Combine(eightballPath, "positive.txt"));
      string rawNegResponses = File.ReadAllText(Path.Combine(eightballPath, "negative.txt"));
      string rawNeutResponses = File.ReadAllText(Path.Combine(eightballPath, "neutral.txt"));

      BallPositiveResponses = rawPosResponses.Split('\n').ToList();
      BallNegativeResponses = rawNegResponses.Split('\n').ToList();
      BallNeutralResponses = rawNeutResponses.Split('\n').ToList();
      BallAllResponses = new List<List<string>>
      {
        BallPositiveResponses,
        BallNegativeResponses,
        BallNeutralResponses,
        BallNeutralResponses//I know, I don't care
      };

      List<string> randList =
        // Get a random 8ball message in advance
        BallAllResponses[ballRandom.Next(BallAllResponses.Count)];

      string rand = randList[ballRandom.Next(randList.Count)];

      string shakingMessage = "*shooka shooka...*";
      int editDelay = 1000;
      if (ballRandom.Next(10) > 8)
      {
        shakingMessage += " **KA-KRASH?!**";
        editDelay += 1500;
      }

      // Setup the initial message
      var message = await this.Context.Channel.SendMessageAsync(shakingMessage);
      // Pretend to think
      await Task.Delay(editDelay);
      // Replace the initial message (edit) with the 8ball message
      await message.ModifyAsync(msg => msg.Content = rand);
    }

    [Command("gender")]
    [Summary("Prints the bots gender identity.")]
    public async Task Gender()
    {
      await this.Context.Channel.SendMessageAsync("🎶 _I'm a **bitch**, I'm a **lover**_ 🎶");
    }

    [Command("vore")]
    [Summary("Vores the given user.")]
    public async Task Vore(
      [Summary("The user to vore")] [Remainder]
      SocketGuildUser user)
    {
      await Vore(user.Nickname);
    }


    [Command("vore")]
    [Summary("Vores the given message.")]
    public async Task Vore(
      [Summary("The message to vore")] [Remainder] string msg)
    {
      if (msg.Length < 3)
      {
        await ReplyAsync("That doesn't look very tasty. No thanks. *(Character minimum of 3 not met.)*");
        return;
      }

      string voreEmoteRaw = "<:turtlevore:585605828283858944>";
      string stareEmoteRaw = "<:turtlestare:585542857218326579>";

      Emote voreEmoji;
      Emote stareEmoji;

      if (!Emote.TryParse(voreEmoteRaw, out voreEmoji) ||
          !Emote.TryParse(stareEmoteRaw, out stareEmoji))
      {
        await ReplyAsync("😢 Looking up the emojis failed...");
        return;
      }

      string lengthValidationCopy = "";
      for (int i = msg.Length; i > 0; i--)
      {
        // TODO: Naive, breaks with multi-coded glyphs. LU C# utf-8 codepoint handling. 
        if (i < 4)
        {
          lengthValidationCopy += "*" + msg.Substring(0, i) + "*" + voreEmoteRaw;
        }
        else
        {
          lengthValidationCopy += msg.Substring(0, i - 3) + "*" + msg.Substring(i - 3, 3) + "*" + voreEmoteRaw;
        }
      }


      if (lengthValidationCopy.Length > 1900) //or so.
      {
        await ReplyAsync("What are you trying to make me vore? Your life story? No thanks. *(Character limit reached.)*");
        return;
      }
      else
      {
        string currMsg = "";
        IUserMessage editableMessage = null;
        for (int i = msg.Length; i > 0; i--)
        {
          // TODO: Naive, breaks with multi-coded glyphs. LU C# utf-8 codepoint handling. 
          if (i < 4)
          {
            currMsg += "*" + msg.Substring(0, i) + "* " + voreEmoji;
          }
          else
          {
            currMsg += msg.Substring(0, i - 3) + "*" + msg.Substring(i - 3, 3) + "* " + voreEmoji;
          }
          currMsg += "\n";

          if(i == msg.Length)
          {
            editableMessage = await ReplyAsync(currMsg);
          }
          else
          {
            await editableMessage.ModifyAsync(msgProps => msgProps.Content = currMsg);
          }

          // put in an arbitrary delay between edits
          await Task.Delay(750);
        }

        currMsg += stareEmoji;
        await editableMessage.ModifyAsync(msgProps => msgProps.Content = currMsg);
      }
    }

  }
}
