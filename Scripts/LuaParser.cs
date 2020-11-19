using System;
using System.Collections.Generic;
using ScriptInspector;
using UnityEngine;

namespace ScriptInspector
{
    public class LuaParser : FGParser
    {
        private static HashSet<string> scriptLiterals = new HashSet<string> {"false", "nil", "true",};

        public override HashSet<string> Keywords {
            get { return keywords; }
        }

        public override HashSet<string> BuiltInLiterals {
            get { return scriptLiterals; }
        }

        public override bool IsBuiltInType(string word) { return builtInTypes.Contains(word); }

        public override bool IsBuiltInLiteral(string word) { return scriptLiterals.Contains(word); }

        private static readonly HashSet<string> keywords = new HashSet<string> {
            "and", "break", "do", "else", "elseif", "end", "false", "for", "function", "if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while"
            , "goto",

            // lua builtin
            "__add"
            , "__band", "__bnot", "__bor", "__bxor", "__call", "__concat", "__div", "__eq", "__gc", "__idiv", "__index", "__le", "__len", "__lt", "__metatable", "__mod", "__mode"
            , "__mul", "__name", "__newindex", "__pairs", "__pow", "__shl", "__shr", "__sub", "__tostring", "__unm",
            //6.1 – Basic Functions
            "type"
            , "assert", "print", "error", "pair", "ipair", "start", "table", "tostring", "require", "self",
            //协程内建
            "coroutine"
            , "create", "isyieldable", "resume", "running", "status", "wrap", "yield",
            //协程拓展
            "start"
            , "yieldstart", "yieldreturn", "yieldcallback", "yieldbreak", "waitforfixedupdate", "waitforframes", "waitforseconds", "waitforasyncop", "waituntil", "waitwhile"
            , "waitforendofframe", "stopwaiting", "startcoroutine", "stopcoroutine",
        };

        private static readonly HashSet<string> operators = new HashSet<string> {
            "+", "-", "*", "/", "%", "^", "#", "&", "~", "|", "<<", ">>", "//", "==", "~=", "<=", ">=", "<", ">", "=", "(", ")", "{", "}", "[", "]", "::", ":", ",", ".", ".."
            , "..."
        };

        private static readonly HashSet<string> builtInTypes = new HashSet<string> {
            "nil", "boolean", "number", "string", "function", "userdata", "thread", "table"
        };

        private static readonly HashSet<string> luaBuildInWords = new HashSet<string> {
            //6.1 – Basic Functions
            "type", "assert", "print", "error", "pair", "ipair", "start", "table", "tostring", "require", "self",
            //协程内建
            "coroutine"
            , "create", "isyieldable", "resume", "running", "status", "wrap", "yield",
            //协程拓展
            "start"
            , "yieldstart", "yieldreturn", "yieldcallback", "yieldbreak", "waitforfixedupdate", "waitforframes", "waitforseconds", "waitforasyncop", "waituntil", "waitwhile"
            , "waitforendofframe", "stopwaiting", "startcoroutine", "stopcoroutine",
        };

        private static readonly HashSet<string> keywordsAndBuiltInTypes;

        static LuaParser() {
            Debug.Log("@zjh LuaParser");
            keywordsAndBuiltInTypes = new HashSet<string>();
            keywordsAndBuiltInTypes.UnionWith(keywords);
            keywordsAndBuiltInTypes.UnionWith(builtInTypes);
            keywordsAndBuiltInTypes.UnionWith(luaBuildInWords);
        }

        public override void LexLine(int currentLine, FGTextBuffer.FormatedLine formatedLine) {
            formatedLine.index = currentLine;

            if (parserThread != null)
                parserThread.Join();
            parserThread = null;

            string textLine = textBuffer.lines[currentLine];


            if (currentLine == 0) {
                var defaultScriptDefines = UnityEditor.EditorUserBuildSettings.activeScriptCompilationDefines;
                if (scriptDefines == null || !scriptDefines.SetEquals(defaultScriptDefines)) {
                    if (scriptDefines == null) {
                        scriptDefines = new HashSet<string>(defaultScriptDefines);
                    } else {
                        scriptDefines.Clear();
                        scriptDefines.UnionWith(defaultScriptDefines);
                    }
                }
            }


            Tokenize(textLine, formatedLine);

            var lineTokens = formatedLine.tokens;

            if (textLine.Length == 0) {
                formatedLine.tokens.Clear();
            } else if (textBuffer.styles != null) {
                var lineWidth = textBuffer.CharIndexToColumn(textLine.Length, currentLine);
                if (lineWidth > textBuffer.longestLine)
                    textBuffer.longestLine = lineWidth;

                for (var i = 0; i < lineTokens.Count; ++i) {
                    var token = lineTokens[i];
                    switch (token.tokenKind) {
                        case SyntaxToken.Kind.Whitespace:
                        case SyntaxToken.Kind.Missing:
                            token.style = textBuffer.styles.normalStyle;
                            break;

                        case SyntaxToken.Kind.Punctuator:
                            token.style = IsOperator(token.text) ? textBuffer.styles.operatorStyle : textBuffer.styles.punctuatorStyle;
                            if (token.text.Equals(":")) {
                                token.style = textBuffer.styles.builtInValueTypeStyle;
                            }

                            break;

                        case SyntaxToken.Kind.Keyword:
                            if (IsBuiltInType(token.text)) {
                                if (token.text == "string") {
                                    token.style = textBuffer.styles.builtInRefTypeStyle;
                                } else {
                                    token.style = textBuffer.styles.builtInValueTypeStyle;
                                }
                            } else if (luaBuildInWords.Contains(token.text)) {
                                if (token.text == "self") {
                                    token.style = textBuffer.styles.builtInLiteralsStyle;
                                } else {
                                    token.style = textBuffer.styles.builtInValueTypeStyle;
                                }
                            } else {
                                token.style = textBuffer.styles.keywordStyle;
                            }

                            break;

                        case SyntaxToken.Kind.Identifier:
                            if (IsBuiltInLiteral(token.text)) {
                                token.style = textBuffer.styles.builtInLiteralsStyle;
                                token.tokenKind = SyntaxToken.Kind.BuiltInLiteral;
                            } else {
                                token.style = textBuffer.styles.normalStyle;
                            }

                            break;

                        case SyntaxToken.Kind.IntegerLiteral:
                        case SyntaxToken.Kind.RealLiteral:
                            token.style = textBuffer.styles.constantStyle;
                            break;

                        case SyntaxToken.Kind.Comment:
                            var regionKind = formatedLine.regionTree.kind;
                            var inactiveLine = regionKind > FGTextBuffer.RegionTree.Kind.LastActive;
                            token.style = inactiveLine ? textBuffer.styles.inactiveCodeStyle : textBuffer.styles.commentStyle;
                            break;

                        case SyntaxToken.Kind.Preprocessor:
                            token.style = textBuffer.styles.preprocessorStyle;
                            break;

                        case SyntaxToken.Kind.PreprocessorSymbol:
                            token.style = textBuffer.styles.defineSymbols;
                            break;

                        case SyntaxToken.Kind.PreprocessorArguments:
                        case SyntaxToken.Kind.PreprocessorCommentExpected:
                        case SyntaxToken.Kind.PreprocessorDirectiveExpected:
                        case SyntaxToken.Kind.PreprocessorUnexpectedDirective:
                            token.style = textBuffer.styles.normalStyle;
                            break;

                        case SyntaxToken.Kind.CharLiteral:
                        case SyntaxToken.Kind.StringLiteral:

                        case SyntaxToken.Kind.VerbatimStringBegin:
                        case SyntaxToken.Kind.VerbatimStringLiteral:
                            token.style = textBuffer.styles.stringStyle;
                            break;
                    }

                    lineTokens[i] = token;
                }
            }
        }

        protected override void Tokenize(string line, FGTextBuffer.FormatedLine formatedLine) {
            var tokens = formatedLine.tokens ?? new List<SyntaxToken>();
            formatedLine.tokens = tokens;
            tokens.Clear();

            int startAt = 0;
            int length = line.Length;
            SyntaxToken token;

            SyntaxToken ws = ScanWhitespace(line, ref startAt);
            if (ws != null) {
                tokens.Add(ws);
                ws.formatedLine = formatedLine;
            }

            var inactiveLine = formatedLine.regionTree.kind > FGTextBuffer.RegionTree.Kind.LastActive;

            while (startAt < length) {
                switch (formatedLine.blockState) {
                    case FGTextBuffer.BlockState.None:
                        ws = ScanWhitespace(line, ref startAt);
                        if (ws != null) {
                            tokens.Add(ws);
                            ws.formatedLine = formatedLine;
                            continue;
                        }

                        if (inactiveLine) {
                            tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt)) {formatedLine = formatedLine});
                            startAt = length;
                            break;
                        }

                        //注释--、注释块开始--[[
                        if (line[startAt] == '-' && startAt < length - 1) {
                            if (startAt + 3 <= length - 1 && line.Substring(startAt, 4) == "--[[") {
                                tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, "--[[") {formatedLine = formatedLine});
                                startAt += 4;
                                formatedLine.blockState = FGTextBuffer.BlockState.CommentBlock;
                                break;
                            } else if (line[startAt + 1] == '-') {
                                tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, "--") {formatedLine = formatedLine});
                                startAt += 2;
                                tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt)) {formatedLine = formatedLine});
                                startAt = length;
                                break;
                            }
                        }

                        if (line[startAt] == '\'') {
                            token = ScanCharLiteral(line, ref startAt);
                            tokens.Add(token);
                            token.formatedLine = formatedLine;
                            break;
                        }

                        if (line[startAt] == '\"') {
                            token = ScanStringLiteral(line, ref startAt);
                            tokens.Add(token);
                            token.formatedLine = formatedLine;
                            break;
                        }

                        if (line[startAt] >= '0' && line[startAt] <= '9'
                            || startAt < length - 1 && line[startAt] == '.' && line[startAt + 1] >= '0' && line[startAt + 1] <= '9') {
                            token = ScanNumericLiteral(line, ref startAt);
                            tokens.Add(token);
                            token.formatedLine = formatedLine;
                            break;
                        }

                        token = ScanIdentifierOrKeyword(line, ref startAt);
                        if (token != null) {
                            tokens.Add(token);
                            token.formatedLine = formatedLine;
                            break;
                        }

                        var punctuatorStart = startAt++;
                        if (startAt < line.Length) {
                            switch (line[punctuatorStart]) {
                                case '?':
                                    if (line[startAt] == '?')
                                        ++startAt;
                                    break;
                                case '+':
                                    if (line[startAt] == '+' || line[startAt] == '=')
                                        ++startAt;
                                    break;
                                case '-':
                                    if (line[startAt] == '-' || line[startAt] == '=')
                                        ++startAt;
                                    break;
                                case '<':
                                    if (line[startAt] == '=')
                                        ++startAt;
                                    else if (line[startAt] == '<') {
                                        ++startAt;
                                        if (startAt < line.Length && line[startAt] == '=')
                                            ++startAt;
                                    }

                                    break;
                                case '>':
                                    if (line[startAt] == '=')
                                        ++startAt;
                                    //else if (startAt < line.Length && line[startAt] == '>')
                                    //{
                                    //    ++startAt;
                                    //    if (line[startAt] == '=')
                                    //        ++startAt;
                                    //}
                                    break;
                                case '=':
                                    if (line[startAt] == '=' || line[startAt] == '>')
                                        ++startAt;
                                    break;
                                case '&':
                                    if (line[startAt] == '=' || line[startAt] == '&')
                                        ++startAt;
                                    break;
                                case '|':
                                    if (line[startAt] == '=' || line[startAt] == '|')
                                        ++startAt;
                                    break;
                                case '*':
                                case '/':
                                case '%':
                                case '^':
                                case '!':
                                    if (line[startAt] == '=')
                                        ++startAt;
                                    break;
                                case ':':
                                    if (line[startAt] == ':')
                                        ++startAt;
                                    break;
                            }
                        }

                        tokens.Add(new SyntaxToken(SyntaxToken.Kind.Punctuator, line.Substring(punctuatorStart, startAt - punctuatorStart)) {formatedLine = formatedLine});
                        break;

                    case FGTextBuffer.BlockState.CommentBlock: //注释
                        int commentBlockEnd = line.IndexOf("]]", startAt, StringComparison.Ordinal);
                        if (commentBlockEnd == -1) {
                            tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt)) {formatedLine = formatedLine});
                            startAt = length;
                        } else {
                            tokens.Add(new SyntaxToken(SyntaxToken.Kind.Comment, line.Substring(startAt, commentBlockEnd + 2 - startAt)) {formatedLine = formatedLine});
                            startAt = commentBlockEnd + 2;
                            formatedLine.blockState = FGTextBuffer.BlockState.None;
                        }

                        break;

                    case FGTextBuffer.BlockState.StringBlock:
                        int i = startAt;
                        int closingQuote = line.IndexOf('\"', startAt);
                        while (closingQuote != -1 && closingQuote < length - 1 && line[closingQuote + 1] == '\"') {
                            i = closingQuote + 2;
                            closingQuote = line.IndexOf('\"', i);
                        }

                        if (closingQuote == -1) {
                            tokens.Add(new SyntaxToken(SyntaxToken.Kind.VerbatimStringLiteral, line.Substring(startAt)) {formatedLine = formatedLine});
                            startAt = length;
                        } else {
                            tokens.Add(new SyntaxToken(SyntaxToken.Kind.VerbatimStringLiteral, line.Substring(startAt, closingQuote - startAt)) {formatedLine = formatedLine});
                            startAt = closingQuote;
                            tokens.Add(new SyntaxToken(SyntaxToken.Kind.VerbatimStringLiteral, line.Substring(startAt, 1)) {formatedLine = formatedLine});
                            ++startAt;
                            formatedLine.blockState = FGTextBuffer.BlockState.None;
                        }

                        break;
                }
            }
        }

        private new SyntaxToken ScanIdentifierOrKeyword(string line, ref int startAt) {
            var token = FGParser.ScanIdentifierOrKeyword(line, ref startAt);
            if (token != null && token.tokenKind == SyntaxToken.Kind.Keyword && !IsKeywordOrBuiltInType(token.text))
                token.tokenKind = SyntaxToken.Kind.Identifier;
            return token;
        }

        private bool IsKeyword(string word) { return keywords.Contains(word); }

        private bool IsKeywordOrBuiltInType(string word) { return keywordsAndBuiltInTypes.Contains(word); }

        private bool IsOperator(string text) { return operators.Contains(text); }
    }
}