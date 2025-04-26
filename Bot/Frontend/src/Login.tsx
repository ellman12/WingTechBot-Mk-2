import { FC, useEffect, useState } from "react";
import http from "./api/http.ts";
import TextField from "./components/TextField.tsx";

interface Props {
    setAuthenticated: (auth: boolean) => void;
}

const Login: FC<Props> = ({setAuthenticated}) => {
    const [userId, setUserId] = useState("");

    function handleLogin() {
        http.post("/auth/login", {user_id: userId})
            .then(e => {
                if (e.statusText === "OK") {
                    setAuthenticated(true);
                    localStorage.setItem("userId", userId);
                } else {
                    setAuthenticated(false);
                }
            })
            .catch(e => console.error(e));
    }

    useEffect(() => {
        const cachedId = localStorage.getItem("userId");
        if (cachedId !== null) {
            setUserId(cachedId);
        }
    }, []);

    return (
        <div className="flex flex-col w-64 gap-6 text-white">
            <TextField value={userId} valueChanged={setUserId} onEnter={handleLogin} placeholder="Enter user ID"/>
            <button onClick={handleLogin} className="bg-blue-500 text-white p-2">Login</button>
        </div>
    );
};

export default Login;
