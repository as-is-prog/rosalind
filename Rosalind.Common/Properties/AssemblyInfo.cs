﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyDescription("")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、このアセンブリ内の型は COM コンポーネントから
// 参照できなくなります。COM からこのアセンブリ内の型にアクセスする必要がある場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// このプロジェクトが COM に公開される場合、次の GUID が typelib の ID になります
[assembly: Guid("12d09bf9-11e4-4c4f-8484-dd56cb89689e")]

// アセンブリのバージョン情報は次の 4 つの値で構成されています:
//
//      メジャー バージョン
//      マイナー バージョン
//      ビルド番号
//      Revision
//
// すべての値を指定するか、次を使用してビルド番号とリビジョン番号を既定に設定できます
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]

// TestクラスからInternalの参照可にする
[assembly: InternalsVisibleTo("Rosalind.Test")]
[assembly: InternalsVisibleTo("Rosalind.NET.Test")]

// 各言語対応プロジェクトからInternalの参照可にする
[assembly: InternalsVisibleTo("Rosalind.CSharp")]
[assembly: InternalsVisibleTo("Rosalind.NET.CSharp")]
[assembly: InternalsVisibleTo("Rosalind.NETCore.CSharp")]
[assembly: InternalsVisibleTo("Rosalind.VisualBasic")]
