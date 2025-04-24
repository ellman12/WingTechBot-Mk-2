import {FC} from "react";
import SoundboardSound from "../api/SoundboardSound.ts";
import http from "../api/http.ts";

interface Props {
    sound: SoundboardSound;
}

const SoundButton: FC<Props> = ({sound}) => {
    async function onClick() {
        await sendSound();
    }

    async function sendSound() {
        await http.post("soundboard/send-soundboard-sound", {guild_id: sound.guild_id, sound_id: sound.sound_id});
    }

    return (
        <button className="flex items-center justify-center bg-[#2B2D31] text-white px-4 py-4 text-nowrap text-sm rounded-lg hover:cursor-pointer" onClick={onClick}>
            {sound.name}
        </button>
    );
};

export default SoundButton;
