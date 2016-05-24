﻿!function (e) { "object" == typeof exports && "object" == typeof module ? e(require("../../lib/codemirror")) : "function" == typeof define && define.amd ? define(["../../lib/codemirror"], e) : e(CodeMirror) }(function (e) { function t(e, t) { return "pairs" == t && "string" == typeof e ? e : "object" == typeof e && null != e[t] ? e[t] : u[t] } function n(e) { return function (t) { return s(t, e) } } function r(e) { var t = e.state.closeBrackets; if (!t) return null; var n = e.getModeAt(e.getCursor()); return n.closeBrackets || t } function i(n) { var i = r(n); if (!i || n.getOption("disableInput")) return e.Pass; for (var a = t(i, "pairs"), o = n.listSelections(), s = 0; s < o.length; s++) { if (!o[s].empty()) return e.Pass; var l = c(n, o[s].head); if (!l || a.indexOf(l) % 2 != 0) return e.Pass } for (var s = o.length - 1; s >= 0; s--) { var f = o[s].head; n.replaceRange("", h(f.line, f.ch - 1), h(f.line, f.ch + 1), "+delete") } } function a(n) { var i = r(n), a = i && t(i, "explode"); if (!a || n.getOption("disableInput")) return e.Pass; for (var o = n.listSelections(), s = 0; s < o.length; s++) { if (!o[s].empty()) return e.Pass; var l = c(n, o[s].head); if (!l || a.indexOf(l) % 2 != 0) return e.Pass } n.operation(function () { n.replaceSelection("\n\n", null), n.execCommand("goCharLeft"), o = n.listSelections(); for (var e = 0; e < o.length; e++) { var t = o[e].head.line; n.indentLine(t, null, !0), n.indentLine(t + 1, null, !0) } }) } function o(t) { var n = e.cmpPos(t.anchor, t.head) > 0; return { anchor: new h(t.anchor.line, t.anchor.ch + (n ? -1 : 1)), head: new h(t.head.line, t.head.ch + (n ? 1 : -1)) } } function s(n, i) { var a = r(n); if (!a || n.getOption("disableInput")) return e.Pass; var s = t(a, "pairs"), c = s.indexOf(i); if (-1 == c) return e.Pass; for (var u, d, g = t(a, "triples"), p = s.charAt(c + 1) == i, v = n.listSelections(), m = c % 2 == 0, b = 0; b < v.length; b++) { var x, C = v[b], P = C.head, d = n.getRange(P, h(P.line, P.ch + 1)); if (m && !C.empty()) x = "surround"; else if (!p && m || d != i) if (p && P.ch > 1 && g.indexOf(i) >= 0 && n.getRange(h(P.line, P.ch - 2), P) == i + i && (P.ch <= 2 || n.getRange(h(P.line, P.ch - 3), h(P.line, P.ch - 2)) != i)) x = "addFour"; else if (p) { if (e.isWordChar(d) || !f(n, P, i)) return e.Pass; x = "both" } else { if (!m || n.getLine(P.line).length != P.ch && !l(d, s) && !/\s/.test(d)) return e.Pass; x = "both" } else x = g.indexOf(i) >= 0 && n.getRange(P, h(P.line, P.ch + 3)) == i + i + i ? "skipThree" : "skip"; if (u) { if (u != x) return e.Pass } else u = x } var S = c % 2 ? s.charAt(c - 1) : i, k = c % 2 ? i : s.charAt(c + 1); n.operation(function () { if ("skip" == u) n.execCommand("goCharRight"); else if ("skipThree" == u) for (var e = 0; 3 > e; e++) n.execCommand("goCharRight"); else if ("surround" == u) { for (var t = n.getSelections(), e = 0; e < t.length; e++) t[e] = S + t[e] + k; n.replaceSelections(t, "around"), t = n.listSelections().slice(); for (var e = 0; e < t.length; e++) t[e] = o(t[e]); n.setSelections(t) } else "both" == u ? (n.replaceSelection(S + k, null), n.triggerElectric(S + k), n.execCommand("goCharLeft")) : "addFour" == u && (n.replaceSelection(S + S + S + S, "before"), n.execCommand("goCharRight")) }) } function l(e, t) { var n = t.lastIndexOf(e); return n > -1 && n % 2 == 1 } function c(e, t) { var n = e.getRange(h(t.line, t.ch - 1), h(t.line, t.ch + 1)); return 2 == n.length ? n : null } function f(t, n, r) { var i = t.getLine(n.line), a = t.getTokenAt(n); if (/\bstring2?\b/.test(a.type)) return !1; var o = new e.StringStream(i.slice(0, n.ch) + r + i.slice(n.ch), 4); for (o.pos = o.start = a.start; ;) { var s = t.getMode().token(o, a.state); if (o.pos >= n.ch + 1) return /\bstring2?\b/.test(s); o.start = o.pos } } var u = { pairs: "()[]{}''\"\"", triples: "", explode: "[]{}" }, h = e.Pos; e.defineOption("autoCloseBrackets", !1, function (t, n, r) { r && r != e.Init && (t.removeKeyMap(g), t.state.closeBrackets = null), n && (t.state.closeBrackets = n, t.addKeyMap(g)) }); for (var d = u.pairs + "`", g = { Backspace: i, Enter: a }, p = 0; p < d.length; p++) g["'" + d.charAt(p) + "'"] = n(d.charAt(p)) });