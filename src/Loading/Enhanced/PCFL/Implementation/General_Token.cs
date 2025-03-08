﻿using System.Text;
using Editor.DataClasses.GameDataClasses;
using Editor.ErrorHandling;
using Editor.Saving;

namespace Editor.Loading.Enhanced.PCFL.Implementation
{
   public abstract class SimpleIntEffect : IToken
   {
      internal Value<int> _value = new(0); // Default value and type of T
      public bool Parse(LineKvp<string, string> command, PathObj po) => GeneralFileParser.ParseSingleTriggerValue(ref _value, command, po, GetTokenName());
      public abstract void Activate(ITarget target);
      public void GetTokenString(int tabs, ref StringBuilder sb) => SavingUtil.AddInt(tabs, _value.Val, GetTokenName(), ref sb);
      public abstract string GetTokenName();
      public abstract string GetTokenDescription();
      public abstract string GetTokenExample();
   }

   public abstract class SimpleStringEffect : IToken
   {
      internal Value<string> _value = new(string.Empty); // Default value and type of T
      public virtual bool Parse(LineKvp<string, string> command, PathObj po) => GeneralFileParser.ParseSingleTriggerValue(ref _value, command, po, GetTokenName());
      public abstract void Activate(ITarget target);
      public void GetTokenString(int tabs, ref StringBuilder sb) => SavingUtil.AddString(tabs, _value.Val, GetTokenName(), ref sb);
      public abstract string GetTokenName();
      public abstract string GetTokenDescription();
      public abstract string GetTokenExample();
   }

   public abstract class SimpleBoolEffect : IToken
   {
      internal Value<bool> _value = new(false); // Default value and type of T
      public bool Parse(LineKvp<string, string> command, PathObj po) => GeneralFileParser.ParseSingleTriggerValue(ref _value, command, po, GetTokenName());
      public abstract void Activate(ITarget target);
      public void GetTokenString(int tabs, ref StringBuilder sb) => SavingUtil.AddBool(tabs, _value.Val, GetTokenName(), ref sb);
      public abstract string GetTokenName();
      public abstract string GetTokenDescription();
      public abstract string GetTokenExample();
   }

   public abstract class SimpleFloatEffect : IToken
   {
      internal Value<float> _value = new(0); // Default value and type of T
      public bool Parse(LineKvp<string, string> command, PathObj po) => GeneralFileParser.ParseSingleTriggerValue(ref _value, command, po, GetTokenName());
      public abstract void Activate(ITarget target);
      public void GetTokenString(int tabs, ref StringBuilder sb) => SavingUtil.AddFloat(tabs, _value.Val, GetTokenName(), ref sb);
      public abstract string GetTokenName();
      public abstract string GetTokenDescription();
      public abstract string GetTokenExample();
   }

   public abstract class SimpleTagEffect : IToken
   {
      internal Value<string> _value = new(string.Empty); // Default value and type of T
      public bool Parse(LineKvp<string, string> command, PathObj po)
      {
         var state = GeneralFileParser.ParseSingleTriggerValue(ref _value, command, po, GetTokenName());
         if(!state)
            return false;
         Tag tag = _value.Val;
         if (!Globals.Countries.TryGetValue(tag, out var country))
         {
            _ = new LoadingError(po, $"Invalid Tag: {tag}", line: command.Line, type: ErrorType.PCFL_TokenValidationError);
            return false;
         }
         _value.Val = country.Tag;
         return true;
      }
      public abstract void Activate(ITarget target);
      public void GetTokenString(int tabs, ref StringBuilder sb) => SavingUtil.AddString(tabs, _value.Val, GetTokenName(), ref sb);
      public abstract string GetTokenName();
      public abstract string GetTokenDescription();
      public abstract string GetTokenExample();
   }
}