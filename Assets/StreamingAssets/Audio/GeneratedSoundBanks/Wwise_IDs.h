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
        static const AkUniqueID PLAY_BASIC_JUMP = 2848448515U;
        static const AkUniqueID PLAY_BEAM_LASER = 1231329179U;
        static const AkUniqueID PLAY_BLOCKED_QUARTZ = 3010479856U;
        static const AkUniqueID PLAY_BLUE_QUARTZ_JUMP = 3128279563U;
        static const AkUniqueID PLAY_CHANGE_QUARTZ_B = 1763377455U;
        static const AkUniqueID PLAY_CHANGE_QUARTZ_G = 1763377450U;
        static const AkUniqueID PLAY_CHANGE_QUARTZ_R = 1763377471U;
        static const AkUniqueID PLAY_CHANGE_QUARTZ_Y = 1763377460U;
        static const AkUniqueID PLAY_CHARGE_SHOOT_LASER = 3751886250U;
        static const AkUniqueID PLAY_DASH_L = 1886667247U;
        static const AkUniqueID PLAY_DASH_R = 1886667249U;
        static const AkUniqueID PLAY_DEATH = 1172822028U;
        static const AkUniqueID PLAY_FOOTSTEPS = 3854155799U;
        static const AkUniqueID PLAY_LANDINGS = 1019246482U;
        static const AkUniqueID PLAY_LASER_LP = 718252718U;
        static const AkUniqueID PLAY_SP_AMB_STEAM = 3893832595U;
        static const AkUniqueID PLAY_STEAMPUNK_MUSIC = 4292865938U;
        static const AkUniqueID PLAY_YELLOW_QUARTZ = 2438634884U;
        static const AkUniqueID SET_STATE_MUSIC_INGAME = 2405722263U;
        static const AkUniqueID SET_STATE_MUSIC_PAUSE = 2666093702U;
        static const AkUniqueID SET_SWITCH_CONCRETE = 3543386712U;
        static const AkUniqueID SET_SWITCH_CRISTAL_PLATFORMS = 4131070040U;
        static const AkUniqueID SET_SWITCH_METAL = 708957530U;
        static const AkUniqueID STOP_LASER_LP = 1386925776U;
        static const AkUniqueID STOP_STEAMPUNK_MUSIC = 2505006364U;
        static const AkUniqueID STOP_YELLOW_QUARTZ = 2588384050U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace GENERAL_GAME_STATE
        {
            static const AkUniqueID GROUP = 2978222038U;

            namespace STATE
            {
                static const AkUniqueID INGAME = 984691642U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PAUSE = 3092587493U;
            } // namespace STATE
        } // namespace GENERAL_GAME_STATE

        namespace STEAMPUNK_MUSIC_STATES
        {
            static const AkUniqueID GROUP = 3322003828U;

            namespace STATE
            {
                static const AkUniqueID INGAME = 984691642U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID PAUSE = 3092587493U;
            } // namespace STATE
        } // namespace STEAMPUNK_MUSIC_STATES

    } // namespace STATES

    namespace SWITCHES
    {
        namespace LEVEL1_MUSIC_SWITCH
        {
            static const AkUniqueID GROUP = 96434573U;

            namespace SWITCH
            {
                static const AkUniqueID DEFAULT = 782826392U;
                static const AkUniqueID PAUSE = 3092587493U;
            } // namespace SWITCH
        } // namespace LEVEL1_MUSIC_SWITCH

        namespace SWITCH_FOOTSTEPS
        {
            static const AkUniqueID GROUP = 3395986341U;

            namespace SWITCH
            {
                static const AkUniqueID CONCRETE = 841620460U;
                static const AkUniqueID CRISTAL_PLATFORMS = 1403804780U;
                static const AkUniqueID METAL = 2473969246U;
            } // namespace SWITCH
        } // namespace SWITCH_FOOTSTEPS

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID DISTANCE = 1240670792U;
        static const AkUniqueID MUSIC_VOLUME = 1006694123U;
        static const AkUniqueID SFX_VOLUME = 1564184899U;
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
