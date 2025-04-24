import {FC, useState} from "react";
import http from "./api/http.ts";

interface Props {
    setAuthenticated: (auth: boolean) => void;
}

const Login: FC<Props> = ({setAuthenticated}) => {
    const [password, setPassword] = useState("");

    function handleLogin() {
        http.post("/auth/login", {password})
            .then(e => {
                if (e.statusText === "OK") {
                    setAuthenticated(true);
                } else {
                    setAuthenticated(false);
                }
            })
            .catch(e => console.error(e));
    }

    return (
        <div className="flex flex-col w-64 gap-4 text-white">
            <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Enter password" className="border p-2"/>
            <button onClick={handleLogin} className="bg-blue-500 text-white p-2">Login</button>
        </div>
    );
};

export default Login;
