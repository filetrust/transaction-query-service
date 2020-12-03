import React from "react";
import SwaggerUI from "swagger-ui-react";
import "swagger-ui-react/swagger-ui.css";
import TopBar from "../shared/TopBar";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

let yam = require("./api.yaml");

process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = "0"

const SwaggerPage = () => {
    return (
        <>
            <TopBar />
            <SwaggerUI url={window.location.origin + yam}
                docExpansion="list"
                defaultModelExpandDepth={1} />
            <ToastContainer />
        </>);
};

export default SwaggerPage;
