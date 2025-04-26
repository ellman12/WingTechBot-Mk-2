import { useState, useEffect } from "react";
import SoundboardSound from "../api/SoundboardSound.ts";
import http from "../api/http.ts";
import SoundButton from "./SoundButton.tsx";
import TextField from "./TextField.tsx";

export default function SoundButtons() {
    const [sounds, setSounds] = useState<SoundboardSound[]>([]);
    const [searchText, setSearchText] = useState("");

    useEffect(() => {
        http.get("soundboard/available-sounds")
            .then(e => setSounds(e.data as SoundboardSound[]))
            .catch(e => console.error(e));
    }, []);

    const lower = searchText.trim().toLowerCase();
    function filter(s: SoundboardSound) {
        if (lower === "")
            return true;

        return s.name.trim().toLowerCase().includes(lower);
    }
    const filteredSounds = sounds.filter(filter);

    return (
        <div className="flex flex-col items-center gap-6">
            <TextField value={searchText} valueChanged={setSearchText} placeholder="Search Sounds" className="w-72"/>

            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
                {sounds?.length > 0 && filteredSounds.map(sound => <SoundButton key={sound.name} sound={sound}/>)}
            </div>
        </div>
    );
};
