// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// https://astro.build/config
export default defineConfig({
	integrations: [
		starlight({
			title: 'Contact Sync',
			favicon: '/ContactSync.svg',
			head: [
				{
					tag: "script",
					attrs: {
						async: true,
						src: "https://www.googletagmanager.com/gtag/js?id=G-B8SY8B2N12",
					}
				},
				{
					tag: "script",
					attrs: {
						src: "/ga.js",
					}
				},
				{
					tag: "meta",
					attrs: {
						property: "og:image",
						content: "https://contact-sync.vrchat.narazaka.net/ContactSync.svg",
					}
				}
			],
			social: [{ icon: 'github', label: 'GitHub', href: 'https://github.com/Narazaka/ContactSync' }],
			sidebar: [
				{
					label: 'Guides',
					autogenerate: { directory: 'guides' },
				},
			],
			locales: {
				root: {
					label: '日本語',
					lang: 'ja',
				}
			}
		}),
	],
});
