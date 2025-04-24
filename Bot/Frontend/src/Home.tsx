import SoundButtons from "./components/SoundButtons.tsx";
import {useState} from "react";
import Login from "./Login.tsx";

export default function Home() {
    const [authenticated, setAuthenticated] = useState(false);

    return (
        <div className="flex flex-col items-center justify-center gap-6 p-8">
            <h1 className="text-2xl text-white">WingTech Bot Mk 2 Soundboard</h1>

            {!authenticated && <Login setAuthenticated={setAuthenticated}/>}

            <div className={authenticated ? "visible" : "hidden"}>
                <SoundButtons/>
            </div>
        </div>
    );
}