
using System;
using System.IO;
using FastCSV;
using FastCSV.Internal;
using FastCSV.Utils;

const string Csv = @"id,first_name,last_name,age,email,gender,ip_address
1,Romeo,Abela,34,rabela0@usatoday.com,Male,217.15.244.208
2,Horatius,Garner,51,hgarner1@163.com,Male,30.236.135.5
3,Noak,Phibb,41,nphibb2@diigo.com,Male,222.23.19.209
4,Sunny,Kuhl,51,skuhl3@1und1.de,Male,170.214.143.198
5,Lucilia,Phinnessy,17,,lphinnessy4@cargocollective.com,Female,154.183.251.5
6,Davidson,Coneybeare,21,dconeybeare5@ow.ly,Male,165.39.63.139
7,Verney,Housbey,35,vhousbey6@alexa.com,Male,175.235.100.95
8,Connor,MacGown,53,cmacgown7@jigsy.com,Male,210.81.178.153
9,Mitchell,Stigell,21,mstigell8@prweb.com,Male,214.174.203.9
10,Raeann,Sincock,60,rsincock9@nhs.uk,Female,207.234.87.146";

var reader = new StreamReader(StreamHelper.CreateStreamFromString(Csv));
var format = new CsvFormat(ignoreNewLine: false, ignoreWhitespaces: false);
using var parser = new CsvParser(reader, format);

while (!parser.IsDone)
{
    string s = string.Join(", ", parser.ParseNext()!);
    Console.WriteLine(s);
}