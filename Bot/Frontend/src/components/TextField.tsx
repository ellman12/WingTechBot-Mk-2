import { FC, KeyboardEvent } from "react";

interface Props {
    value: string;
    valueChanged: (newText: string) => void;
    onEnter?: () => void;
    type?: string;
    placeholder: string;
    className?: string;
}

const TextField: FC<Props> = ({value, valueChanged, onEnter, type = "text", placeholder, className = ""}) => {
    function handleKeyDown(e: KeyboardEvent) {
        if (e.key === "Enter" && onEnter) {
            onEnter();
        }
    }

    return (
        <input id="sound-search" type={type} value={value} placeholder={placeholder} onChange={e => valueChanged(e.target.value)} onKeyDown={handleKeyDown} className={`text-white border-2 rounded-lg border-white outline-blue-500 focus:outline-2 focus:border-blue-500 p-2 ${className}`}/>
    );
};

export default TextField;
