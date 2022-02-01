/*
 * Copyright (C) 2005-2022 SplendidCRM Software, Inc. 
 * MIT License
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
 * modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 */

// 1. React and fabric. 
import * as React from 'react';

// https://infinum.com/the-capsized-eight/how-to-use-react-hooks-in-class-components
function useScreenSize(): any
{
	const [width , setWidth ] = React.useState(window.innerWidth );
	const [height, setHeight] = React.useState(window.innerHeight);

	React.useEffect(() =>
	{
		const handler = (event: any) =>
		{
			setWidth (event.target.innerWidth );
			setHeight(event.target.innerHeight);
		};
		window.addEventListener('resize', handler);
		return () =>
		{
			window.removeEventListener('resize', handler);
		};
	}, []);

	return {width, height};
}

const withScreenSizeHook = (Component: any) =>
{
	return (props: any) =>
	{
		const screenSize = useScreenSize();
		return <Component screenSize={ screenSize } {...props} />;
	};
};

export default withScreenSizeHook;

