import {useState, useEffect} from "react";
import SoundboardSound from "../api/SoundboardSound.ts";
import http from "../api/http.ts";
import SoundButton from "./SoundButton.tsx";

export default function SoundButtons() {
    const [sounds, setSounds] = useState<SoundboardSound[]>([]);

    useEffect(() => {
        http.get("soundboard/available-sounds")
            .then(e => setSounds(e.data.sounds as SoundboardSound[]))
            .catch(e => console.error(e));
    }, []);

    return (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
            {sounds?.length > 0 && sounds.map(sound => <SoundButton key={sound.sound_id} sound={sound}/>)}
        </div>
    );
};
