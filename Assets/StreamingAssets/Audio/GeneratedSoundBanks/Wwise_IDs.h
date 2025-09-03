/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID IDLE_START = 2452979090U;
        static const AkUniqueID PLAY_BASIC_JUMP = 2848448515U;
        static const AkUniqueID PLAY_BEAM_LASER = 1231329179U;
        static const AkUniqueID PLAY_BLOCKED_QUARTZ = 3010479856U;
        static const AkUniqueID PLAY_BLUE_QUARTZ_JUMP = 3128279563U;
        static const AkUniqueID PLAY_BUZZ_HIT = 2938714251U;
        static const AkUniqueID PLAY_BUZZ_SAW = 3077319667U;
        static const AkUniqueID PLAY_CHANGE_QUARTZ_B = 1763377455U;
        static const AkUniqueID PLAY_CHANGE_QUARTZ_G = 1763377450U;
        static const AkUniqueID PLAY_CHANGE_QUARTZ_R = 1763377471U;
        static const AkUniqueID PLAY_CHANGE_QUARTZ_Y = 1763377460U;
        static const AkUniqueID PLAY_CHARGE_SHOOT_LASER = 3751886250U;
        static const AkUniqueID PLAY_DASH_L = 1886667247U;
        static const AkUniqueID PLAY_DASH_R = 1886667249U;
        static const AkUniqueID PLAY_DEATH = 1172822028U;
        static const AkUniqueID PLAY_DX_MOLLY_STANDARD = 3684740494U;
        static const AkUniqueID PLAY_FOOTSTEPS = 3854155799U;
        static const AkUniqueID PLAY_HIT_CHAINS = 2170076162U;
        static const AkUniqueID PLAY_LANDINGS = 1019246482U;
        static const AkUniqueID PLAY_LASER_LP = 718252718U;
        static const AkUniqueID PLAY_LUCIERNAGAS = 3912385716U;
        static const AkUniqueID PLAY_MX_SWITCH = 2855120456U;
        static const AkUniqueID PLAY_SFX_STARTANIMATION = 751341016U;
        static const AkUniqueID PLAY_SP_AMB_STEAM = 3893832595U;
        static const AkUniqueID PLAY_STEAM_SPRING = 358772238U;
        static const AkUniqueID PLAY_YELLOW_QUARTZ = 2438634884U;
        static const AkUniqueID SET_MXSWITCH_ADVENTURER = 1662526074U;
        static const AkUniqueID SET_MXSWITCH_GHOST = 2922058705U;
        static const AkUniqueID SET_MXSWITCH_MAINTHEME = 4039992738U;
        static const AkUniqueID SET_MXSWITCH_NOMX = 52117186U;
        static const AkUniqueID SET_STATE_MUSIC_INGAME = 2405722263U;
        static const AkUniqueID SET_STATE_MUSIC_PAUSE = 2666093702U;
        static const AkUniqueID SET_STATE_MUSICPUZZLE_OFF = 2818173839U;
        static const AkUniqueID SET_STATE_MUSICPUZZLE_ON = 368556571U;
        static const AkUniqueID SET_SWITCH_CONCRETE = 3543386712U;
        static const AkUniqueID SET_SWITCH_CRISTAL_PLATFORMS = 4131070040U;
        static const AkUniqueID SET_SWITCH_METAL = 708957530U;
        static const AkUniqueID STOP_LASER_LP = 1386925776U;
        static const AkUniqueID STOP_LUCIERNAGAS = 119388346U;
        static const AkUniqueID STOP_MX_SWITCH = 3636185906U;
        static const AkUniqueID STOP_STEAMPUNK_MUSIC = 2505006364U;
        static const AkUniqueID STOP_YELLOW_QUARTZ = 2588384050U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace LUCIERNAGAS
        {
            static const AkUniqueID GROUP = 2466252489U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID OFF = 930712164U;
                static const AkUniqueID ON = 1651971902U;
            } // namespace STATE
        } // namespace LUCIERNAGAS

        namespace MUSICPUZZLESTATE
        {
            static const AkUniqueID GROUP = 4281606737U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID OFF = 930712164U;
                static const AkUniqueID ON = 1651971902U;
            } // namespace STATE
        } // namespace MUSICPUZZLESTATE

        namespace MUSICSTATE
        {
            static const AkUniqueID GROUP = 1021618141U;

            namespace STATE
            {
                static const AkUniqueID INGAME = 984691642U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PAUSE = 3092587493U;
            } // namespace STATE
        } // namespace MUSICSTATE

    } // namespace STATES

    namespace SWITCHES
    {
        namespace FOOTSTEPSSWITCH
        {
            static const AkUniqueID GROUP = 3586861854U;

            namespace SWITCH
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID CRISTAL_PLATFORMS = 1403804780U;
                static const AkUniqueID METAL = 2473969246U;
            } // namespace SWITCH
        } // namespace FOOTSTEPSSWITCH

        namespace MUSICSWITCH
        {
            static const AkUniqueID GROUP = 1445037870U;

            namespace SWITCH
            {
                static const AkUniqueID ADVENTURER = 3247222823U;
                static const AkUniqueID AQUATIC = 3653896563U;
                static const AkUniqueID GHOST = 4023194814U;
                static const AkUniqueID MAINTHEME = 824317709U;
                static const AkUniqueID NOMX = 732117587U;
            } // namespace SWITCH
        } // namespace MUSICSWITCH

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID RTPC_DISTANCE = 262290038U;
        static const AkUniqueID RTPC_LUCIERNAGAS = 1704512779U;
        static const AkUniqueID RTPC_MUSICVOLUME = 2378823330U;
        static const AkUniqueID RTPC_SAWSONSCREEN = 1354283134U;
        static const AkUniqueID RTPC_SFXVOLUME = 2644490154U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMBS = 3537656742U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID PLAYER = 1069431850U;
        static const AkUniqueID SFX_S = 989987316U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID LARGEROOM = 187046019U;
        static const AkUniqueID REVERBS = 3545700988U;
        static const AkUniqueID SMALLROOM = 2933838247U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
