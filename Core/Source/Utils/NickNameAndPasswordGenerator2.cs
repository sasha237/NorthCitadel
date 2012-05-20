using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Security.Cryptography;


namespace NerZul.Core.Utils
{
    public class NickNameAndPasswordGenerator2
    {

        private System.Random m_Random = new System.Random();
        public string GenerateNick()
        {
            string rv = "";


            var thefirstname = "";
            var thelastname = "";

            var h_fnpre = new Hashtable();
            var h_fnsuf = new Hashtable();
            var h_lnpre = new Hashtable();
            var h_lnsuf = new Hashtable();

            var o_fnpre = new Hashtable();
            var o_fnsuf = new Hashtable();

            h_fnpre[0] = "Te";
            h_fnpre[1] = "Ni";
            h_fnpre[2] = "Nila";
            h_fnpre[3] = "Andro";
            h_fnpre[4] = "Androma";
            h_fnpre[5] = "Sha";
            h_fnpre[6] = "Ara";
            h_fnpre[7] = "Ma";
            h_fnpre[8] = "Mana";
            h_fnpre[9] = "La";
            h_fnpre[10] = "Landa";
            h_fnpre[11] = "Do";
            h_fnpre[12] = "Dori";
            h_fnpre[13] = "Pe";
            h_fnpre[14] = "Peri";
            h_fnpre[15] = "Conju";
            h_fnpre[16] = "Co";
            h_fnpre[17] = "Fo";
            h_fnpre[18] = "Fordre";
            h_fnpre[19] = "Da";
            h_fnpre[20] = "Dala";
            h_fnpre[21] = "Ke";
            h_fnpre[22] = "Kele";
            h_fnpre[23] = "Gra";
            h_fnpre[24] = "Grani";
            h_fnpre[25] = "Jo";
            h_fnpre[26] = "Sa";
            h_fnpre[27] = "Mala";
            h_fnpre[28] = "Ga";
            h_fnpre[29] = "Gavi";
            h_fnpre[30] = "Gavinra";
            h_fnpre[31] = "Mo";
            h_fnpre[32] = "Morlu";
            h_fnpre[33] = "Aga";
            h_fnpre[34] = "Agama";
            h_fnpre[35] = "Ba";
            h_fnpre[36] = "Balla";
            h_fnpre[37] = "Ballado";
            h_fnpre[38] = "Za";
            h_fnpre[39] = "Ari";
            h_fnpre[40] = "Ariu";
            h_fnpre[41] = "Au";
            h_fnpre[42] = "Auri";
            h_fnpre[43] = "Bra";
            h_fnpre[44] = "Ka";
            h_fnpre[45] = "Bu";
            h_fnpre[46] = "Buza";
            h_fnpre[47] = "Coi";
            h_fnpre[48] = "Bo";
            h_fnpre[49] = "Mu";
            h_fnpre[50] = "Muni";
            h_fnpre[51] = "Tho";
            h_fnpre[52] = "Thorga";
            h_fnpre[53] = "Ke";
            h_fnpre[54] = "Gri";
            h_fnpre[55] = "Bu";
            h_fnpre[56] = "Buri";
            h_fnpre[57] = "Hu";
            h_fnpre[58] = "Hugi";
            h_fnpre[59] = "Tho";
            h_fnpre[60] = "Thordi";
            h_fnpre[61] = "Ba";
            h_fnpre[62] = "Bandi";
            h_fnpre[63] = "Ga";
            h_fnpre[64] = "Bea";
            h_fnpre[65] = "Beaze";
            h_fnpre[66] = "Mo";
            h_fnpre[67] = "Modi";
            h_fnpre[68] = "Ma";
            h_fnpre[69] = "Malo";
            h_fnpre[70] = "Gholbi";
            h_fnpre[71] = "Gho";
            h_fnpre[72] = "Da";
            h_fnpre[73] = "Dagda";
            h_fnpre[74] = "Nua";
            h_fnpre[75] = "Nuada";
            h_fnpre[76] = "Oghma";
            h_fnpre[77] = "Ce";
            h_fnpre[78] = "Centri";
            h_fnpre[79] = "Cere";
            h_fnpre[80] = "Ce";
            h_fnpre[81] = "Ka";
            h_fnpre[82] = "Kathri";
            h_fnpre[83] = "Ado";
            h_fnpre[84] = "Adora";
            h_fnpre[85] = "Mora";
            h_fnpre[86] = "Mo";
            h_fnpre[87] = "Fe";
            h_fnpre[88] = "Felo";
            h_fnpre[89] = "Ana";
            h_fnpre[90] = "Anara";
            h_fnpre[91] = "Kera";
            h_fnpre[92] = "Mave";
            h_fnpre[93] = "Dela";
            h_fnpre[94] = "Mira";
            h_fnpre[95] = "Theta";
            h_fnpre[96] = "Tygra";
            h_fnpre[97] = "Adrie";
            h_fnpre[98] = "Diana";
            h_fnpre[99] = "Alsa";
            h_fnpre[100] = "Mari";
            h_fnpre[101] = "Shali";
            h_fnpre[102] = "Sira";
            h_fnpre[103] = "Sai";
            h_fnpre[104] = "Saithi";
            h_fnpre[105] = "Mala";
            h_fnpre[106] = "Kiri";
            h_fnpre[107] = "Ana";
            h_fnpre[108] = "Anaya";
            h_fnpre[109] = "Felha";
            h_fnpre[110] = "Drela";
            h_fnpre[111] = "Corda";
            h_fnpre[112] = "Nalme";
            h_fnpre[113] = "Na";
            h_fnpre[114] = "Um";
            h_fnpre[115] = "Ian";
            h_fnpre[116] = "Opi";
            h_fnpre[117] = "Lai";
            h_fnpre[118] = "Ygg";
            h_fnpre[119] = "Mne";
            h_fnpre[120] = "Ishn";
            h_fnpre[121] = "Kula";
            h_fnpre[122] = "Yuni";



            h_fnsuf[1] = "nn";
            h_fnsuf[2] = "las";
            h_fnsuf[4] = "math";
            h_fnsuf[5] = "th";
            h_fnsuf[7] = "ath";
            h_fnsuf[8] = "zar";
            h_fnsuf[9] = "ril";
            h_fnsuf[10] = "ris";
            h_fnsuf[11] = "rus";
            h_fnsuf[12] = "jurus";
            h_fnsuf[13] = "dred";
            h_fnsuf[14] = "rdred";
            h_fnsuf[15] = "lar";
            h_fnsuf[16] = "len";
            h_fnsuf[17] = "nis";
            h_fnsuf[18] = "rn";
            h_fnsuf[19] = "ge";
            h_fnsuf[20] = "lak";
            h_fnsuf[21] = "nrad";
            h_fnsuf[22] = "rad";
            h_fnsuf[23] = "lune";
            h_fnsuf[24] = "kus";
            h_fnsuf[25] = "mand";
            h_fnsuf[26] = "gamand";
            h_fnsuf[27] = "llador";
            h_fnsuf[28] = "dor";
            h_fnsuf[29] = "dar";
            h_fnsuf[30] = "nadar";
            h_fnsuf[31] = "rius";
            h_fnsuf[32] = "nius";
            h_fnsuf[33] = "zius";
            h_fnsuf[34] = "tius";
            h_fnsuf[35] = "sius";
            h_fnsuf[36] = "wield";
            h_fnsuf[37] = "helm";
            h_fnsuf[38] = "zan";
            h_fnsuf[39] = "tus";
            h_fnsuf[40] = "bor";
            h_fnsuf[41] = "nin";
            h_fnsuf[42] = "rgas";
            h_fnsuf[43] = "gas";
            h_fnsuf[44] = "lv";
            h_fnsuf[45] = "kelv";
            h_fnsuf[46] = "gelv";
            h_fnsuf[47] = "rim";
            h_fnsuf[48] = "sida";
            h_fnsuf[49] = "ginn";
            h_fnsuf[50] = "grinn";
            h_fnsuf[51] = "nn";
            h_fnsuf[52] = "huginn";
            h_fnsuf[53] = "rdin";
            h_fnsuf[54] = "ndis";
            h_fnsuf[55] = "bandis";
            h_fnsuf[56] = "gar";
            h_fnsuf[57] = "zel";
            h_fnsuf[58] = "di";
            h_fnsuf[59] = "ron";
            h_fnsuf[60] = "rne";
            h_fnsuf[61] = "lbine";
            h_fnsuf[62] = "gda";
            h_fnsuf[63] = "ghma";
            h_fnsuf[64] = "ntrius";
            h_fnsuf[65] = "dwyn";
            h_fnsuf[66] = "wyn";
            h_fnsuf[67] = "swyn";
            h_fnsuf[68] = "thris";
            h_fnsuf[69] = "dora";
            h_fnsuf[70] = "lore";
            h_fnsuf[71] = "nara";
            h_fnsuf[72] = "ra";
            h_fnsuf[73] = "las";
            h_fnsuf[74] = "gra";
            h_fnsuf[75] = "riel";
            h_fnsuf[76] = "lsa";
            h_fnsuf[77] = "rin";
            h_fnsuf[78] = "lis";
            h_fnsuf[79] = "this";
            h_fnsuf[80] = "lace";
            h_fnsuf[81] = "ri";
            h_fnsuf[82] = "naya";
            h_fnsuf[83] = "rana";
            h_fnsuf[84] = "lhala";
            h_fnsuf[85] = "lanim";
            h_fnsuf[86] = "rdana";
            h_fnsuf[87] = "lmeena";
            h_fnsuf[88] = "meena";
            h_fnsuf[89] = "fym";
            h_fnsuf[90] = "fyn";
            h_fnsuf[91] = "hara";

            h_lnpre[0] = "Flame";
            h_lnpre[1] = "Arcane";
            h_lnpre[2] = "Light";
            h_lnpre[3] = "Mage";
            h_lnpre[4] = "Spell";
            h_lnpre[5] = "Rex";
            h_lnpre[6] = "Dawn";
            h_lnpre[7] = "Dark";
            h_lnpre[8] = "Red";
            h_lnpre[9] = "Truth";
            h_lnpre[10] = "Might";
            h_lnpre[11] = "True";
            h_lnpre[12] = "Bright";
            h_lnpre[13] = "Pure";
            h_lnpre[14] = "Fearless";
            h_lnpre[15] = "Dire";
            h_lnpre[16] = "Blue";
            h_lnpre[17] = "White";
            h_lnpre[18] = "Black";
            h_lnpre[19] = "Rain";
            h_lnpre[20] = "Doom";
            h_lnpre[21] = "Rune";
            h_lnpre[22] = "Sword";
            h_lnpre[23] = "Force";
            h_lnpre[24] = "Axe";
            h_lnpre[25] = "Stone";
            h_lnpre[26] = "Iron";
            h_lnpre[27] = "Broad";
            h_lnpre[28] = "Stern";
            h_lnpre[29] = "Thunder";
            h_lnpre[30] = "Frost";
            h_lnpre[31] = "Rock";
            h_lnpre[32] = "Doom";
            h_lnpre[33] = "Blud";
            h_lnpre[34] = "Blood";
            h_lnpre[35] = "Stone";
            h_lnpre[36] = "Steel";
            h_lnpre[37] = "Golden";
            h_lnpre[38] = "Gold";
            h_lnpre[39] = "Silver";
            h_lnpre[40] = "White";
            h_lnpre[41] = "Black";
            h_lnpre[42] = "Gravel";
            h_lnpre[43] = "Sharp";
            h_lnpre[44] = "Star";
            h_lnpre[45] = "Night";
            h_lnpre[46] = "Moon";
            h_lnpre[47] = "Chill";
            h_lnpre[48] = "Whisper";
            h_lnpre[49] = "White";
            h_lnpre[50] = "Black";
            h_lnpre[51] = "Saber";
            h_lnpre[52] = "Snow";
            h_lnpre[53] = "Rain";
            h_lnpre[54] = "Dark";
            h_lnpre[55] = "Light";
            h_lnpre[56] = "Wind";
            h_lnpre[57] = "Iron";
            h_lnpre[58] = "Blade";
            h_lnpre[59] = "Shadow";
            h_lnpre[60] = "Flame";
            h_lnpre[61] = "Sin";
            h_lnpre[62] = "Pain";
            h_lnpre[63] = "Hell";
            h_lnpre[64] = "Wrath";
            h_lnpre[65] = "Rage";
            h_lnpre[66] = "Blood";
            h_lnpre[67] = "Terror";

            h_lnsuf[0] = "seeker";
            h_lnsuf[1] = "caster";
            h_lnsuf[2] = "binder";
            h_lnsuf[3] = "weaver";
            h_lnsuf[4] = "singer";
            h_lnsuf[5] = "font";
            h_lnsuf[6] = "hammer";
            h_lnsuf[7] = "redeemer";
            h_lnsuf[8] = "bearer";
            h_lnsuf[9] = "bringer";
            h_lnsuf[10] = "defender";
            h_lnsuf[11] = "conjuror";
            h_lnsuf[12] = "eye";
            h_lnsuf[13] = "staff";
            h_lnsuf[14] = "flame";
            h_lnsuf[15] = "fire";
            h_lnsuf[16] = "shaper";
            h_lnsuf[17] = "breaker";
            h_lnsuf[18] = "cliff";
            h_lnsuf[19] = "worm";
            h_lnsuf[20] = "hammer";
            h_lnsuf[21] = "brew";
            h_lnsuf[22] = "beard";
            h_lnsuf[23] = "fire";
            h_lnsuf[24] = "forge";
            h_lnsuf[25] = "stone";
            h_lnsuf[26] = "smith";
            h_lnsuf[27] = "fist";
            h_lnsuf[28] = "pick";
            h_lnsuf[29] = "skin";
            h_lnsuf[30] = "smasher";
            h_lnsuf[31] = "crusher";
            h_lnsuf[32] = "worker";
            h_lnsuf[33] = "shaper";
            h_lnsuf[34] = "song";
            h_lnsuf[35] = "shade";
            h_lnsuf[36] = "singer";
            h_lnsuf[37] = "ray";
            h_lnsuf[38] = "wind";
            h_lnsuf[39] = "fang";
            h_lnsuf[40] = "dragon";
            h_lnsuf[41] = "mane";
            h_lnsuf[42] = "scar";
            h_lnsuf[43] = "moon";
            h_lnsuf[44] = "wood";
            h_lnsuf[45] = "raven";
            h_lnsuf[46] = "wing";
            h_lnsuf[47] = "hunter";
            h_lnsuf[48] = "warden";
            h_lnsuf[49] = "stalker";
            h_lnsuf[50] = "grove";
            h_lnsuf[51] = "walker";
            h_lnsuf[52] = "master";
            h_lnsuf[53] = "blade";
            h_lnsuf[54] = "fury";
            h_lnsuf[55] = "weaver";
            h_lnsuf[56] = "terror";
            h_lnsuf[57] = "dweller";
            h_lnsuf[58] = "killer";
            h_lnsuf[59] = "seeker";
            h_lnsuf[60] = "bourne";
            h_lnsuf[61] = "bringer";
            h_lnsuf[62] = "runner";
            h_lnsuf[63] = "brand";
            h_lnsuf[64] = "wrath";

            o_fnpre[0] = "To";
            o_fnpre[1] = "Toja";
            o_fnpre[2] = "Ni";
            o_fnpre[3] = "Niko";
            o_fnpre[4] = "Ka";
            o_fnpre[5] = "Kaji";
            o_fnpre[6] = "Mi";
            o_fnpre[7] = "Mika";
            o_fnpre[8] = "Sa";
            o_fnpre[9] = "Samu";
            o_fnpre[10] = "Aki";
            o_fnpre[11] = "Akino";
            o_fnpre[12] = "Ma";
            o_fnpre[13] = "Mazu";
            o_fnpre[14] = "Yo";
            o_fnpre[15] = "Yozshu";
            o_fnpre[16] = "Da";
            o_fnpre[17] = "Dai";
            o_fnpre[18] = "Ki";
            o_fnpre[19] = "Kiga";
            o_fnpre[20] = "Ara";
            o_fnpre[21] = "Arashi";
            o_fnpre[22] = "Mo";
            o_fnpre[23] = "Moogu";
            o_fnpre[24] = "Ju";
            o_fnpre[25] = "Ga";
            o_fnpre[26] = "Garda";
            o_fnpre[27] = "Ne";
            o_fnpre[28] = "Ka";
            o_fnpre[29] = "Ma";
            o_fnpre[30] = "Ba";
            o_fnpre[31] = "Go";
            o_fnpre[32] = "Kaga";
            o_fnpre[33] = "Na";
            o_fnpre[34] = "Mo";
            o_fnpre[35] = "Kazra";
            o_fnpre[36] = "Kazi";
            o_fnpre[37] = "Fe";
            o_fnpre[38] = "Fenri";
            o_fnpre[39] = "Ma";
            o_fnpre[40] = "Tygo";
            o_fnpre[41] = "Ta";
            o_fnpre[42] = "Du";
            o_fnpre[43] = "Ka";
            o_fnpre[44] = "Ke";
            o_fnpre[45] = "Mu";
            o_fnpre[46] = "Gro";
            o_fnpre[47] = "Me";
            o_fnpre[48] = "Mala";
            o_fnpre[49] = "Tau";
            o_fnpre[50] = "Te";
            o_fnpre[51] = "Tu";
            o_fnpre[52] = "Mau";
            o_fnpre[53] = "Zu";
            o_fnpre[54] = "Zulki";
            o_fnpre[55] = "JoJo";
            o_fnpre[56] = "Sha";
            o_fnpre[57] = "Shaka";
            o_fnpre[58] = "Shakti";
            o_fnpre[59] = "Me";
            o_fnpre[60] = "Mezi";
            o_fnpre[61] = "Mezti";
            o_fnpre[62] = "Vo";
            o_fnpre[63] = "Do";
            o_fnpre[64] = "Du";
            o_fnpre[65] = "Di";
            o_fnpre[66] = "Vu";
            o_fnpre[67] = "Vi";
            o_fnpre[68] = "Dou";
            o_fnpre[69] = "Ga";
            o_fnpre[70] = "Gu";
            o_fnpre[71] = "Fae";
            o_fnpre[72] = "Fau";
            o_fnpre[73] = "Go";
            o_fnpre[74] = "Golti";
            o_fnpre[75] = "Vudo";
            o_fnpre[76] = "Voodoo";
            o_fnpre[77] = "Zolo";
            o_fnpre[78] = "Zulu";
            o_fnpre[79] = "Bra";
            o_fnpre[80] = "Net";


            o_fnsuf[0] = "jora";
            o_fnsuf[1] = "kora";
            o_fnsuf[2] = "jind";
            o_fnsuf[3] = "kasa";
            o_fnsuf[4] = "muro";
            o_fnsuf[5] = "nos";
            o_fnsuf[6] = "kinos";
            o_fnsuf[7] = "zuru";
            o_fnsuf[8] = "zshura";
            o_fnsuf[9] = "shura";
            o_fnsuf[10] = "ra";
            o_fnsuf[11] = "sho";
            o_fnsuf[12] = "gami";
            o_fnsuf[13] = "mi";
            o_fnsuf[14] = "shicage";
            o_fnsuf[15] = "cage";
            o_fnsuf[16] = "gul";
            o_fnsuf[17] = "bei";
            o_fnsuf[18] = "dal";
            o_fnsuf[19] = "gal";
            o_fnsuf[20] = "zil";
            o_fnsuf[21] = "gis";
            o_fnsuf[22] = "le";
            o_fnsuf[23] = "rr";
            o_fnsuf[24] = "gar";
            o_fnsuf[25] = "gor";
            o_fnsuf[26] = "grel";
            o_fnsuf[27] = "rg";
            o_fnsuf[28] = "gore";
            o_fnsuf[29] = "zragore";
            o_fnsuf[30] = "nris";
            o_fnsuf[31] = "sar";
            o_fnsuf[32] = "risar";
            o_fnsuf[33] = "rn";
            o_fnsuf[34] = "gore";
            o_fnsuf[35] = "m";
            o_fnsuf[36] = "rn";
            o_fnsuf[37] = "t";
            o_fnsuf[38] = "ll";
            o_fnsuf[39] = "k";
            o_fnsuf[40] = "lar";
            o_fnsuf[41] = "r";
            o_fnsuf[42] = "taur";
            o_fnsuf[43] = "taxe";
            o_fnsuf[44] = "lkis";
            o_fnsuf[45] = "labar";
            o_fnsuf[46] = "bar";
            o_fnsuf[47] = "jas";
            o_fnsuf[48] = "lrajas";
            o_fnsuf[49] = "lmaran";
            o_fnsuf[50] = "ran";
            o_fnsuf[51] = "kazahn";
            o_fnsuf[52] = "zahn";
            o_fnsuf[53] = "hn";
            o_fnsuf[54] = "lar";
            o_fnsuf[55] = "tilar";
            o_fnsuf[56] = "ktilar";
            o_fnsuf[57] = "zilkree";
            o_fnsuf[58] = "kree";
            o_fnsuf[59] = "lkree";
            o_fnsuf[60] = "jin";
            o_fnsuf[61] = "jinn";
            o_fnsuf[62] = "shakar";
            o_fnsuf[63] = "jar";
            o_fnsuf[64] = "ramar";
            o_fnsuf[65] = "kus";
            o_fnsuf[66] = "sida";
            o_fnsuf[67] = "Worm";
            switch(m_Random.Next(1,3))
            {
                case 1:
                    {

                        var fnprefix1 = m_Random.Next(122);
                        var fnsuffix1 = m_Random.Next(91);
                        var lnprefix1 = m_Random.Next(67);
                        var lnsuffix1 = m_Random.Next(64);

                        thefirstname = h_fnpre[fnprefix1].ToString() + h_fnsuf[fnsuffix1].ToString();
                        thelastname = h_lnpre[lnprefix1].ToString() + h_lnsuf[lnsuffix1].ToString();

                        var thefirstname1 = thefirstname.Substring(0, 1).ToUpper();
                        thefirstname = thefirstname1 + thefirstname.Substring(1, thefirstname.Length-1);

                        var thelastname1 = thelastname.Substring(0, 1).ToUpper();
                        thelastname = thelastname1 + thelastname.Substring(1, thelastname.Length-1);

                    }
                    break;
                case 2:
                    {

                        var fnprefix1 = m_Random.Next(80);
                        var fnsuffix1 = m_Random.Next(67);

                        thefirstname = o_fnpre[fnprefix1].ToString() + o_fnsuf[fnsuffix1].ToString();
                        thelastname = "";

                    }
                    break;
                case 3:
                    {

                        var fnprefix1 = m_Random.Next(122);
                        var fnsuffix1 = m_Random.Next(91);

                        thefirstname = h_fnpre[fnprefix1].ToString() + h_fnsuf[fnsuffix1].ToString();

                    }
                    break;
            }
            rv = thefirstname + " " + thelastname;
            return rv.Trim();
        }

        public string GenerateGMail(string login)
        {
            string rv = "";
            for (int c = 0; c < login.Length; c++)
            {
                rv += login[c];
                if ((c + 1) != login.Length)
                {
                    int rnd = m_Random.Next(0, 30000);
                    if (rnd > 15000)
                        rv += ".";
                }
            }
            return rv + "@gmail.com";
        }

        //пижжено отседа http://www.codeproject.com/KB/cs/pwdgen.aspx
        public string GeneratePassword()
        {
            // Pick random length between minimum and maximum   

            int pwdLength = GetCryptographicRandomNumber(this.Minimum,
                this.Maximum);

            StringBuilder pwdBuffer = new StringBuilder();
            pwdBuffer.Capacity = this.Maximum;

            // Generate random characters

            char lastCharacter, nextCharacter;

            // Initial dummy character flag

            lastCharacter = nextCharacter = '\n';

            for (int i = 0; i < pwdLength; i++)
            {
                nextCharacter = GetRandomCharacter();

                if (false == this.ConsecutiveCharacters)
                {
                    while (lastCharacter == nextCharacter)
                    {
                        nextCharacter = GetRandomCharacter();
                    }
                }

                if (false == this.RepeatCharacters)
                {
                    string temp = pwdBuffer.ToString();
                    int duplicateIndex = temp.IndexOf(nextCharacter);
                    while (-1 != duplicateIndex)
                    {
                        nextCharacter = GetRandomCharacter();
                        duplicateIndex = temp.IndexOf(nextCharacter);
                    }
                }

                if ((null != this.Exclusions))
                {
                    while (-1 != this.Exclusions.IndexOf(nextCharacter))
                    {
                        nextCharacter = GetRandomCharacter();
                    }
                }

                pwdBuffer.Append(nextCharacter);
                lastCharacter = nextCharacter;
            }

            if (null != pwdBuffer)
            {
                return pwdBuffer.ToString();
            }
            else
            {
                return String.Empty;
            }	
        }

        public NickNameAndPasswordGenerator2() 
        {
            this.Minimum               = DefaultMinimum;
            this.Maximum               = DefaultMaximum;
            this.ConsecutiveCharacters = false;
            this.RepeatCharacters      = true;
            this.ExcludeSymbols        = false;
            this.Exclusions            = null;

            rng = new RNGCryptoServiceProvider();
        }		
		
        protected int GetCryptographicRandomNumber(int lBound, int uBound)
        {   
            // Assumes lBound >= 0 && lBound < uBound

            // returns an int >= lBound and < uBound

            uint urndnum;   
            byte[] rndnum = new Byte[4];   
            if (lBound == uBound-1)  
            {
                // test for degenerate case where only lBound can be returned

                return lBound;
            }
                                                              
            uint xcludeRndBase = (uint.MaxValue -
                (uint.MaxValue%(uint)(uBound-lBound)));   
            
            do 
            {      
                rng.GetBytes(rndnum);      
                urndnum = System.BitConverter.ToUInt32(rndnum,0);      
            } while (urndnum >= xcludeRndBase);   
            
            return (int)(urndnum % (uBound-lBound)) + lBound;
        }

        protected char GetRandomCharacter()
        {            
            int upperBound = pwdCharArray.GetUpperBound(0);

            if ( true == this.ExcludeSymbols )
            {
                upperBound = UBoundDigit;
            }

            int randomCharPosition = GetCryptographicRandomNumber(
                pwdCharArray.GetLowerBound(0), upperBound);

            char randomChar = pwdCharArray[randomCharPosition];

            return randomChar;
        }

        public string Exclusions
        {
            get { return this.exclusionSet; }
            set { this.exclusionSet = value; }
        }

        public int Minimum
        {
            get { return this.minSize; }
            set
            {
                this.minSize = value;
                if (DefaultMinimum > this.minSize)
                {
                    this.minSize = DefaultMinimum;
                }
            }
        }

        public int Maximum
        {
            get { return this.maxSize; }
            set
            {
                this.maxSize = value;
                if (this.minSize >= this.maxSize)
                {
                    this.maxSize = DefaultMaximum;
                }
            }
        }

        public bool ExcludeSymbols
        {
            get { return this.hasSymbols; }
            set { this.hasSymbols = value; }
        }

        public bool RepeatCharacters
        {
            get { return this.hasRepeating; }
            set { this.hasRepeating = value; }
        }

        public bool ConsecutiveCharacters
        {
            get { return this.hasConsecutive; }
            set { this.hasConsecutive = value; }
        }

        private const int DefaultMinimum = 6;
        private const int DefaultMaximum = 10;
        private const int UBoundDigit = 61;

        private RNGCryptoServiceProvider rng;
        private int minSize;
        private int maxSize;
        private bool hasRepeating;
        private bool hasConsecutive;
        private bool hasSymbols;
        private string exclusionSet;
        private char[] pwdCharArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?".ToCharArray();                
    }
}
