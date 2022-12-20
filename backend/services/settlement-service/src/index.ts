
import * as dotenv from "dotenv";
import express from "express";
dotenv.config();

import Bugsnag from '@bugsnag/js'
import cors from "cors";
import { buildingsRoutes } from "./routers/buildings.routes";
import { configRoutes } from "./routers/config.routes";
import { avatarRoutes } from "./routers/avatar.routes";
import { ftRoutes as ftRoutes } from "./routers/ft.routes";
import { questsRoutes } from "./routers/quests.routes";
Bugsnag.start("58948bf7047baa1e478933a5b57cb36a");


if (!process.env.PORT) {
    process.exit(1);
}

const app = express();

app.use(
    cors({
        origin: (origin, callback) => callback(null, true),
        credentials: true,
    })
);
app.use(express.json());

app.use("/api/buildings", buildingsRoutes);
app.use("/api/config", configRoutes);

app.use("/api/avatar", avatarRoutes);
app.use("/api/ft", ftRoutes);
app.use("/api/quests", questsRoutes);

app.get('/healthcheck', (req, res) => {
    res.status(200).json({});
});

app.listen(process.env.PORT, async () => {
    console.log(`Listening on port ${process.env.PORT}`);
});