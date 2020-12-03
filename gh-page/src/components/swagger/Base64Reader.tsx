const _readInputFileBuffer = async (file: File): Promise<ArrayBuffer> => {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsArrayBuffer(file);
        reader.onload = () => {
            if (typeof reader.result === "string") {
                resolve(Buffer.from(reader.result, 0, reader.result.length));
            }
            else {
                resolve(reader.result);
            }
        };
        reader.onerror = error => {
            reject(error);
        };
    });
};

const _arrayBufferToBase64 = (buffer: ArrayBuffer) => {
    const bytes = new Uint8Array(buffer);
    const len = bytes.byteLength;

    let binary = "";
    for (let i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }

    return window.btoa(binary);
};

class Base64Reader {
    static ReadFromInput = async (input: HTMLInputElement) => {
        if (!input || !input.files || !input.files.length) {
            return "";
        }

        const file = input.files[0];
        const fileBuffer = await _readInputFileBuffer(file);
        return _arrayBufferToBase64(fileBuffer);
    }
};

export default Base64Reader;